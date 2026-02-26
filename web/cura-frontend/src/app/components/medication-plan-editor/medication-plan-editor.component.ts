import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CdkDragDrop, CdkDrag, CdkDropList, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { MedicationPlanService } from '../../services/medication-plan.service';
import { Medication } from '../../models/medication.model';
import { MedicationPlan } from '../../models/medication-plan.model';

interface MedicationItem {
  id: number;
  name: string;
  quantity: number;
  isNew: boolean;
}

interface TimeSlot {
  id: string;
  label: string;
  flag: number;
  medications: MedicationItem[];
}

interface WeekdayPlan {
  label: string;
  flag: number;
  timeSlots: TimeSlot[];
}

@Component({
  selector: 'app-medication-plan-editor',
  imports: [CommonModule, FormsModule, CdkDrag, CdkDropList],
  templateUrl: './medication-plan-editor.component.html',
  styleUrl: './medication-plan-editor.component.css'
})
export class MedicationPlanEditorComponent implements OnInit {
  availableMedications: Medication[] = [];
  weekdayPlans: WeekdayPlan[] = [];
  
  newMedicationName: string = '';
  showAddMedicationForm: boolean = false;
  patientId: number = 1; // TODO: Get from auth service
  
  // Plan Gültigkeit
  validFrom: string = new Date().toISOString().split('T')[0]; // YYYY-MM-DD
  validTo: string = ''; // Leer = unbegrenzt
  
  weekdays = [
    { label: 'Montag', flag: 2 },
    { label: 'Dienstag', flag: 4 },
    { label: 'Mittwoch', flag: 8 },
    { label: 'Donnerstag', flag: 16 },
    { label: 'Freitag', flag: 32 },
    { label: 'Samstag', flag: 64 },
    { label: 'Sonntag', flag: 1 }
  ];

  timeSlotTemplates = [
    { id: 'morning', label: 'Morgens', flag: 1 },
    { id: 'noon', label: 'Mittags', flag: 2 },
    { id: 'evening', label: 'Abends', flag: 8 },
    { id: 'night', label: 'Nachts', flag: 16 }
  ];

  constructor(
    private medicationPlanService: MedicationPlanService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadAvailableMedications();
    this.initializeWeekdayPlans();
    this.loadExistingPlan();
  }

  goBack() {
    this.router.navigate(['/']);
  }

  initializeWeekdayPlans() {
    this.weekdayPlans = this.weekdays.map(day => ({
      label: day.label,
      flag: day.flag,
      timeSlots: this.timeSlotTemplates.map(slot => ({
        id: `${day.label.toLowerCase()}-${slot.id}`,
        label: slot.label,
        flag: slot.flag,
        medications: []
      }))
    }));
  }

  loadAvailableMedications() {
    this.medicationPlanService.getAllMedications().subscribe({
      next: (meds) => {
        this.availableMedications = meds;
      },
      error: (err) => console.error('Fehler beim Laden der Medikamente:', err)
    });
  }

  loadExistingPlan() {
    const today = new Date().toISOString();
    this.medicationPlanService.getMedicationPlans(this.patientId).subscribe({
      next: (plans) => {
        // Nur aktive Pläne, die aktuell gültig sind
        const activePlans = plans.filter(p => {
          const validFrom = new Date(p.validFrom);
          const validTo = p.validTo ? new Date(p.validTo) : null;
          const now = new Date();
          
          return p.isActive && 
                 validFrom <= now && 
                 (!validTo || validTo >= now);
        });
        
        console.log('� Geladene Pläne (alle):', plans.length);
        console.log('📥 Aktive Pläne:', activePlans.length);
        
        // DEDUPLIZIERUNG: Gruppiere und kombiniere Pläne
        const dedupMap = new Map<string, MedicationPlan>();
        activePlans.forEach(p => {
          const key = `${p.medicationId}_${p.dayTimeFlags}_${p.quantity}`;
          
          if (dedupMap.has(key)) {
            // Kombiniere weekdayFlags (OR-Verknüpfung)
            const existing = dedupMap.get(key)!;
            existing.weekdayFlags |= p.weekdayFlags;
          } else {
            // Neuer Eintrag
            dedupMap.set(key, { ...p });
          }
        });
        
        const deduplicatedPlans = Array.from(dedupMap.values());
        console.log('📥 Nach Deduplizierung:', deduplicatedPlans.length, 'Pläne');
        
        this.populatePlanFromBackend(deduplicatedPlans);
      },
      error: (err) => console.error('Fehler beim Laden des Plans:', err)
    });
  }

  populatePlanFromBackend(plans: MedicationPlan[]) {
    // Zuerst alle Slots leeren
    this.weekdayPlans.forEach(dayPlan => {
      dayPlan.timeSlots.forEach(slot => {
        slot.medications = [];
      });
    });

    // Pläne sind bereits dedupliziert, einfach einfügen
    plans.forEach(plan => {
      const medication = this.availableMedications.find(m => m.id === plan.medicationId);
      if (!medication) {
        console.warn('⚠️ Medikament nicht gefunden:', plan.medicationId);
        return;
      }

      const medItem: MedicationItem = {
        id: medication.id,
        name: medication.name,
        quantity: plan.quantity,
        isNew: false
      };

      // Für jeden Wochentag der im Plan enthalten ist
      this.weekdayPlans.forEach(dayPlan => {
        const hasDay = (plan.weekdayFlags & dayPlan.flag) !== 0;
        if (!hasDay) return;

        // Für jede Tageszeit die im Plan enthalten ist
        dayPlan.timeSlots.forEach(slot => {
          const hasTime = (plan.dayTimeFlags & slot.flag) !== 0;
          if (hasTime) {
            slot.medications.push({ ...medItem });
          }
        });
      });
    });
    
    console.log('✅ Pläne in UI geladen');
  }

  drop(event: CdkDragDrop<any>) {
    if (event.previousContainer === event.container) {
      // Innerhalb des gleichen Slots umsortieren
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      // Von Medikamentenliste in Slot ziehen
      const sourceItem = event.previousContainer.data[event.previousIndex];
      
      // Konvertiere Medication zu MedicationItem
      const newItem: MedicationItem = {
        id: sourceItem.id,
        name: sourceItem.name,
        quantity: 1,
        isNew: sourceItem.id < 0
      };
      
      // Prüfe ob Medikament bereits im Slot ist
      const exists = event.container.data.find((m: MedicationItem) => m.id === newItem.id);
      if (!exists) {
        event.container.data.push(newItem);
        console.log('Medikament hinzugefügt:', newItem.name, 'zu Slot');
      } else {
        console.log('Medikament bereits vorhanden:', newItem.name);
      }
    }
  }

  addNewMedication() {
    if (!this.newMedicationName.trim()) return;

    // Erstelle temporäres Medikament mit negativer ID
    const newMed: Medication = {
      id: -(this.availableMedications.length + 1),
      name: this.newMedicationName.trim(),
      activeIngredient: ''
    };

    this.availableMedications.push(newMed);
    this.newMedicationName = '';
    this.showAddMedicationForm = false;
  }

  removeMedication(slot: TimeSlot, index: number) {
    slot.medications.splice(index, 1);
  }

  updateQuantity(med: MedicationItem, newQuantity: number) {
    med.quantity = Math.max(1, newQuantity);
  }

  savePlan() {
    console.log('💾 Speichere neuen Medikamentenplan...');

    // Validierung: ValidFrom muss gesetzt sein
    if (!this.validFrom) {
      alert('⚠️ Bitte geben Sie ein Start-Datum an!');
      return;
    }

    // Validierung: ValidTo muss nach ValidFrom liegen
    if (this.validTo && new Date(this.validTo) < new Date(this.validFrom)) {
      alert('⚠️ End-Datum muss nach dem Start-Datum liegen!');
      return;
    }

    // Schritt 1: Sammle alle Medikamente und gruppiere nach Medikament + Tageszeit
    const medicationMap = new Map<string, {
      medicationId: number;
      medicationName: string;
      dayTimeFlags: number;
      weekdayFlags: number;
      quantity: number;
      isNew: boolean;
    }>();

    this.weekdayPlans.forEach(dayPlan => {
      dayPlan.timeSlots.forEach(slot => {
        slot.medications.forEach(med => {
          const key = `${med.id}_${slot.flag}`;
          
          if (medicationMap.has(key)) {
            // Füge Wochentag hinzu
            const entry = medicationMap.get(key)!;
            entry.weekdayFlags |= dayPlan.flag;
          } else {
            // Neuer Eintrag
            medicationMap.set(key, {
              medicationId: med.id,
              medicationName: med.name,
              dayTimeFlags: slot.flag,
              weekdayFlags: dayPlan.flag,
              quantity: med.quantity,
              isNew: med.id < 0
            });
          }
        });
      });
    });

    const plans = Array.from(medicationMap.values());
    
    if (plans.length === 0) {
      alert('⚠️ Bitte fügen Sie mindestens ein Medikament hinzu!');
      return;
    }

    console.log('📋 Zu speichernde Pläne:', plans);
    console.log('📅 Gültig von:', this.validFrom, 'bis:', this.validTo || 'unbegrenzt');

    // Schritt 2: Lösche ALLE alten Pläne (einfacher und funktioniert garantiert)
    console.log('🗑️ Lösche alle alten Pläne für Patient', this.patientId);
    this.medicationPlanService.deleteAllPlansForPatient(this.patientId).subscribe({
      next: () => {
        console.log('✅ Alte Pläne gelöscht');
        
        // Schritt 3: Erstelle neue Pläne
        const validToDate = this.validTo || new Date(2099, 11, 31).toISOString().split('T')[0];
        const plansToCreate: Partial<MedicationPlan>[] = plans.map(p => ({
          patientId: this.patientId,
          medicationId: p.medicationId > 0 ? p.medicationId : undefined,
          caregiverId: 1,
          weekdayFlags: p.weekdayFlags,
          dayTimeFlags: p.dayTimeFlags,
          quantity: p.quantity,
          validFrom: this.validFrom + 'T00:00:00',
          validTo: validToDate + 'T23:59:59',
          notes: p.isNew ? `Neues Medikament: ${p.medicationName}` : '',
          isActive: true
        }));

        console.log('💾 Erstelle neue Pläne:', plansToCreate);
        
        this.medicationPlanService.createMedicationPlans(plansToCreate).subscribe({
          next: (created) => {
            console.log('✅ Pläne erfolgreich gespeichert:', created);
            const dateRange = this.validTo 
              ? `${this.formatDate(this.validFrom)} bis ${this.formatDate(this.validTo)}`
              : `ab ${this.formatDate(this.validFrom)}`;
            alert(`✅ Medikamentenplan gespeichert!\n\nGültig: ${dateRange}\n${plans.length} Einträge erstellt.`);
            this.goBack();
          },
          error: (err) => {
            console.error('❌ Fehler beim Erstellen der Pläne:', err);
            alert('❌ Fehler beim Speichern des Plans!\nBitte versuchen Sie es erneut.');
          }
        });
      },
      error: (err) => {
        console.error('❌ Fehler beim Löschen alter Pläne:', err);
        alert('❌ Fehler beim Löschen alter Pläne!\n\nBitte kontaktieren Sie den Administrator.');
      }
    });
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('de-DE', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }

  copyDayPlan(fromDay: WeekdayPlan) {
    const toIndices = prompt(`In welche Tage kopieren? (z.B. 2,3,5 für Di,Mi,Fr):\n1=Mo, 2=Di, 3=Mi, 4=Do, 5=Fr, 6=Sa, 7=So`);
    if (!toIndices) return;

    const indices = toIndices.split(',').map(s => parseInt(s.trim()) - 1);
    
    indices.forEach(idx => {
      if (idx >= 0 && idx < 7) {
        const targetDay = this.weekdayPlans[idx];
        targetDay.timeSlots.forEach((targetSlot, slotIdx) => {
          targetSlot.medications = [...fromDay.timeSlots[slotIdx].medications.map(m => ({ ...m }))];
        });
      }
    });
  }

  getDropListIds(): string[] {
    const ids: string[] = [];
    this.weekdayPlans.forEach(day => {
      day.timeSlots.forEach(slot => {
        ids.push(slot.id);
      });
    });
    return ids;
  }
}
