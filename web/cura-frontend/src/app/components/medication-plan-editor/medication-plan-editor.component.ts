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
    this.medicationPlanService.getMedicationPlans(this.patientId).subscribe({
      next: (plans) => {
        this.populatePlanFromBackend(plans);
      },
      error: (err) => console.error('Fehler beim Laden des Plans:', err)
    });
  }

  populatePlanFromBackend(plans: MedicationPlan[]) {
    plans.forEach(plan => {
      const medication = this.availableMedications.find(m => m.id === plan.medicationId);
      if (!medication) return;

      const medItem: MedicationItem = {
        id: medication.id,
        name: medication.name,
        quantity: plan.quantity,
        isNew: false
      };

      // Für jeden Wochentag
      this.weekdayPlans.forEach(dayPlan => {
        const hasDay = (plan.weekdayFlags & dayPlan.flag) !== 0;
        if (!hasDay) return;

        // Für jede Tageszeit
        dayPlan.timeSlots.forEach(slot => {
          const hasTime = (plan.dayTimeFlags & slot.flag) !== 0;
          if (hasTime) {
            // Prüfe ob Medikament bereits existiert
            const exists = slot.medications.find(m => m.id === medItem.id);
            if (!exists) {
              slot.medications.push({ ...medItem });
            }
          }
        });
      });
    });
  }

  drop(event: CdkDragDrop<MedicationItem[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      // Kopiere statt verschieben (aus Medikamentenliste)
      const item = event.previousContainer.data[event.previousIndex];
      const newItem: MedicationItem = { ...item, quantity: 1 };
      
      // Prüfe ob Medikament bereits im Slot ist
      const exists = event.container.data.find(m => m.id === item.id);
      if (!exists) {
        event.container.data.splice(event.currentIndex, 0, newItem);
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
    const plans: Partial<MedicationPlan>[] = [];

    this.weekdayPlans.forEach(dayPlan => {
      dayPlan.timeSlots.forEach(slot => {
        slot.medications.forEach(med => {
          // Suche ob Plan bereits existiert
          let existingPlan = plans.find(p => 
            p.medicationId === med.id && 
            (p.dayTimeFlags! & slot.flag) !== 0
          );

          if (existingPlan) {
            // Füge Wochentag hinzu
            existingPlan.weekdayFlags! |= dayPlan.flag;
          } else {
            // Erstelle neuen Plan
            plans.push({
              patientId: this.patientId,
              medicationId: med.id > 0 ? med.id : undefined,
              caregiverId: 0,
              weekdayFlags: dayPlan.flag,
              dayTimeFlags: slot.flag,
              quantity: med.quantity,
              validFrom: new Date().toISOString(),
              validTo: '',
              notes: med.isNew ? `Neu: ${med.name}` : '',
              isActive: true
            });
          }
        });
      });
    });

    console.log('Speichere Pläne:', plans);

    // TODO: API Call zum Speichern
    // 1. Neue Medikamente erstellen (mit negativer ID)
    // 2. Alte Pläne löschen
    // 3. Neue Pläne erstellen
    
    alert(`${plans.length} Medikamentenpläne wurden erstellt!`);
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
}
