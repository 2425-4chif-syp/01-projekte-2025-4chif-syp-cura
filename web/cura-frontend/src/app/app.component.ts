import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { KeycloakService } from 'keycloak-angular';
import { CalendarDay } from './models/calendar-day.model';
import { MedicationPlan } from './models/medication-plan.model';
import { DayDetail } from './models/day-detail.model';
import { CalendarService } from './services/calendar.service';
import { MedicationPlanService } from './services/medication-plan.service';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'cura-frontend';
  currentDate = new Date().toLocaleDateString('de-DE', { day: '2-digit', month: '2-digit', year: '2-digit' });
  currentMonth = '';
  currentDayName = ''; // Für mobile Ansicht (z.B. "Montag")
  currentDayIndex = 0; // Index für aktuellen Wochentag (0=Mo, 6=So)
  userName: string = 'User';
  userRoles: string[] = [];
  currentPatientId: number = 1; // ID des angemeldeten Patienten
  
  calendarDays: CalendarDay[] = [];
  medicationPlans: MedicationPlan[] = [];
  medicationRows: { timeLabel: string; days: string[] }[] = [];
  intakeQuote = 85;
  checkedPercentage = 0;
  partialPercentage = 0;
  missedPercentage = 0;
  
  // Day Detail View
  showDayDetail = false;
  selectedDay: CalendarDay | null = null;
  selectedDayMedications: { timeLabel: string; medication: string; status: 'taken' | 'missed' | 'unknown' }[] = [];
  
  // Medication Plan Flip
  showPlanSelection = false;
  selectedPlanId: string = ''; // Jetzt String (Datum im Format YYYY-MM-DD)
  selectedPlanName: string = 'Mein Medikamentenplan';
  availablePlans: { id: string; name: string; patientName: string; validFrom: string; medicationCount: number }[] = [];
  groupedMedications: { timeLabel: string; medications: { name: string; status: 'taken' | 'missed' }[]; allTaken: boolean }[] = [];
  expandedTimeGroups = new Set<string>();
  
  // Mobile Carousel
  currentTimeIndex = 0;
  touchStartX = 0;
  touchEndX = 0;

  // Create Plan Wizard
  showCreateWizard = false;
  currentWizardStep = 0;
  wizardSteps = ['Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag', 'Sonntag', 'Zusammenfassung'];
  
  availableMedications: Array<{ id: number; name: string }> = [];
  
  // Neue Struktur: weekSchedule[dayIndex][timeOfDay] = [medications]
  // dayIndex: 0-6 (Mo-So), timeOfDay: MORNING, NOON, AFTERNOON, EVENING
  weekSchedule: Map<number, Map<string, Array<{
    medicationId: number | null;
    name: string;
    dosage: number;
    dosageUnit: string;
  }>>> = new Map();
  
  // Separates Formular für jede Tageszeit
  currentMedications: { [key: string]: { medicationId: number | null, name: string, dosage: number, dosageUnit: string } } = {
    'MORNING': { medicationId: null, name: '', dosage: 1, dosageUnit: 'Tablette(n)' },
    'NOON': { medicationId: null, name: '', dosage: 1, dosageUnit: 'Tablette(n)' },
    'AFTERNOON': { medicationId: null, name: '', dosage: 1, dosageUnit: 'Tablette(n)' },
    'EVENING': { medicationId: null, name: '', dosage: 1, dosageUnit: 'Tablette(n)' }
  };
  
  timesOfDay = ['MORNING', 'NOON', 'AFTERNOON', 'EVENING'];
  timeLabels: { [key: string]: string } = {
    'MORNING': 'Morgen',
    'NOON': 'Mittag',
    'AFTERNOON': 'Nachmittag',
    'EVENING': 'Abend'
  };
  
  weekdays = [
    { value: 2, name: 'Montag', short: 'Mo', color: '#FF5252' },
    { value: 4, name: 'Dienstag', short: 'Di', color: '#FF9800' },
    { value: 8, name: 'Mittwoch', short: 'Mi', color: '#FFEB3B' },
    { value: 16, name: 'Donnerstag', short: 'Do', color: '#4CAF50' },
    { value: 32, name: 'Freitag', short: 'Fr', color: '#2196F3' },
    { value: 64, name: 'Samstag', short: 'Sa', color: '#9C27B0' },
    { value: 1, name: 'Sonntag', short: 'So', color: '#795548' }
  ];

  constructor(
    private keycloak: KeycloakService,
    private calendarService: CalendarService,
    private medicationPlanService: MedicationPlanService
  ) {}

  async ngOnInit() {
    // Aktuellen Wochentag berechnen
    const now = new Date();
    const dayNames = ['Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag', 'Sonntag'];
    const dayIndex = now.getDay(); // 0=Sonntag, 1=Montag, ..., 6=Samstag
    // Konvertiere zu Mo=0, Di=1, ..., So=6
    this.currentDayIndex = dayIndex === 0 ? 6 : dayIndex - 1;
    this.currentDayName = dayNames[this.currentDayIndex];

    // User-Info von Keycloak laden
    try {
      const profile = await this.keycloak.loadUserProfile();
      this.userName = profile.firstName || 'User';
      this.userRoles = this.keycloak.getUserRoles();
      
      // Patient-ID aus Keycloak-Attributen holen (falls vorhanden)
      const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
      if (tokenParsed && tokenParsed['patientId']) {
        this.currentPatientId = parseInt(tokenParsed['patientId'], 10);
      }
      console.log('Angemeldeter Patient ID:', this.currentPatientId);
    } catch (error) {
      console.error('Fehler beim Laden des User-Profils:', error);
    }

    // Rest des Codes
    this.currentMonth = this.calendarService.getCurrentMonth();
    this.loadCalendar();
    this.loadAvailablePlans();
    this.loadMedicationPlans();
    this.loadAvailableMedications();
  }

  logout() {
    this.keycloak.logout(window.location.origin);
  }

  // Medication Plan Selection
  loadAvailablePlans() {
    // Lade nur Pläne für den aktuellen Patienten
    this.availablePlans = [
      { id: this.currentPatientId, name: 'Mein Medikamentenplan', patientName: this.userName }
    ];
    this.selectedPlanId = this.currentPatientId;
    this.loadMedicationPlans();
  }

  loadAvailableMedications() {
    this.medicationPlanService.getAllMedications().subscribe({
      next: (medications) => {
        this.availableMedications = medications.map(m => ({ id: m.id, name: m.name }));
      },
      error: (error) => {
        console.error('Fehler beim Laden der Medikamente:', error);
        this.availableMedications = [];
      }
    });
  }

  openPlanSelection() {
    this.showPlanSelection = true;
  }

  closePlanSelection() {
    this.showPlanSelection = false;
  }

  selectPlan(planId: string) {
    this.selectedPlanId = planId;
    const plan = this.availablePlans.find(p => p.id === planId);
    if (plan) {
      this.selectedPlanName = plan.name;
    }
    this.closePlanSelection();
    // Lade Medikamentenpläne für den ausgewählten Plan neu
    this.loadMedicationPlanData(planId);
    this.loadCalendar();
  }
  }

  loadCalendar() {
    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth() + 1;
    const patientId = this.selectedPlanId || 1;

    this.calendarService.getDailyStatus(patientId, year, month).subscribe({
      next: (statusData) => {
        this.calendarDays = this.calendarService.generateCalendarFromStatus(statusData);
        this.intakeQuote = this.calendarService.calculateIntakeQuote(statusData);
        this.calculateStatusPercentages();
      },
      error: () => {
        this.calendarDays = this.calendarService.generateEmptyCalendar();
        this.intakeQuote = 0;
        this.checkedPercentage = 0;
        this.partialPercentage = 0;
        this.missedPercentage = 0;
      }
    });
  }

  loadMedicationPlans() {
    // Lade Plan-Gruppen für den Patienten
    this.medicationPlanService.getPatientPlanGroups(this.currentPatientId).subscribe({
      next: (planGroups) => {
        this.availablePlans = planGroups;
        
        // Wähle neuesten Plan automatisch aus, wenn noch keiner gewählt
        if (!this.selectedPlanId && planGroups.length > 0) {
          this.selectedPlanId = planGroups[0].id;
          this.selectedPlanName = planGroups[0].name;
        }
        
        // Lade die Medikamente für den ausgewählten Plan
        if (this.selectedPlanId) {
          this.loadMedicationPlanData(this.selectedPlanId);
        }
      },
      error: (error) => {
        console.error('Fehler beim Laden der Plan-Gruppen:', error);
        this.medicationRows = this.medicationPlanService.buildMedicationTable([], new Map());
      }
    });
  }

  loadMedicationPlanData(planDateId: string) {
    this.medicationPlanService.getMedicationPlansByDate(this.currentPatientId, planDateId).subscribe({
      next: (plans) => {
        this.medicationPlans = plans;
        this.loadMedicationNames(plans);
      },
      error: (error) => {
        console.error('Fehler beim Laden der Medikamentenpläne:', error);
        this.medicationRows = this.medicationPlanService.buildMedicationTable([], new Map());
      }
    });
  }

  loadMedicationNames(plans: MedicationPlan[]) {
    const uniqueMedicationIds = [...new Set(plans.map(p => p.medicationId))];
    const medicationRequests = uniqueMedicationIds.map(id => 
      this.medicationPlanService.getMedicationName(id)
    );

    if (medicationRequests.length === 0) {
      this.medicationRows = this.medicationPlanService.buildMedicationTable(plans, new Map());
      return;
    }

    import('rxjs').then(({ forkJoin }) => {
      forkJoin(medicationRequests).subscribe({
        next: (names) => {
          const medicationNames = new Map<number, string>();
          uniqueMedicationIds.forEach((id, index) => {
            medicationNames.set(id, names[index]);
          });
          this.medicationRows = this.medicationPlanService.buildMedicationTable(plans, medicationNames);
        },
        error: () => {
          this.medicationRows = this.medicationPlanService.buildMedicationTable(plans, new Map());
        }
      });
    });
  }

  calculateStatusPercentages() {
    const totalDays = this.calendarDays.filter(d => d.day > 0).length;
    if (totalDays === 0) {
      this.checkedPercentage = 0;
      this.partialPercentage = 0;
      this.missedPercentage = 0;
      return;
    }

    const checkedDays = this.calendarDays.filter(d => d.checked).length;
    const partialDays = this.calendarDays.filter(d => d.partial).length;
    const missedDays = this.calendarDays.filter(d => d.missed).length;

    this.checkedPercentage = Math.round((checkedDays / totalDays) * 100);
    this.missedPercentage = Math.round((missedDays / totalDays) * 100) + Math.round((partialDays / totalDays) * 100);
  }

  openDayDetail(day: CalendarDay) {
    if (day.day === 0) return;
    
    this.selectedDay = day;
    this.showDayDetail = true;
    this.loadDayMedications(day.date);
  }

  closeDayDetail() {
    this.showDayDetail = false;
    this.selectedDay = null;
    this.selectedDayMedications = [];
  }

  loadDayMedications(date: string) {
    // Parse date (format: YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS)
    const dateOnly = date.split('T')[0]; // Remove time part if present
    const [year, month, day] = dateOnly.split('-').map(Number);
    const patientId = this.selectedPlanId || 1;
    
    // Load all scheduled and taken medications for this day
    this.medicationPlanService.getDayDetails(patientId, year, month, day).subscribe({
      next: (details) => {
        console.log('Day details from backend:', details);
        
        // Convert to display format
        this.selectedDayMedications = details.map(detail => {
          let medication = detail.medicationName;
          
          if (detail.wasTaken && detail.intakeTime) {
            const time = new Date(detail.intakeTime).toLocaleTimeString('de-DE', { 
              hour: '2-digit', 
              minute: '2-digit' 
            });
            medication += ` - ${time}`;
          } else {
            medication;
          }
          
          return {
            timeLabel: detail.timeLabel,
            medication: medication,
            status: detail.wasTaken ? 'taken' as const : 'missed' as const
          };
        });
        
        console.log('Selected day medications:', this.selectedDayMedications);
        
        // Group by time
        this.groupMedicationsByTime();
      },
      error: (err) => {
        console.error('Error loading day medications:', err);
        this.selectedDayMedications = [];
        this.groupedMedications = [];
      }
    });
  }

  groupMedicationsByTime() {
    const timeOrder = ['Morning', 'Noon', 'Afternoon', 'Evening'];
    const medicationsByTime = new Map<string, { name: string; status: 'taken' | 'missed' }[]>();
    
    // Reset expanded groups
    this.expandedTimeGroups.clear();
    
    // Gruppiere nach Tageszeit
    for (const med of this.selectedDayMedications) {
      if (!medicationsByTime.has(med.timeLabel)) {
        medicationsByTime.set(med.timeLabel, []);
      }
      // Filter out 'unknown' status
      if (med.status !== 'unknown') {
        medicationsByTime.get(med.timeLabel)!.push({
          name: med.medication,
          status: med.status
        });
      }
    }
    
    // Erstelle sortiertes Array
    this.groupedMedications = timeOrder
      .filter(time => medicationsByTime.has(time))
      .map(time => {
        const medications = medicationsByTime.get(time)!;
        const allTaken = medications.every(m => m.status === 'taken');
        
        // Auto-expand wenn nicht alle genommen wurden
        if (!allTaken) {
          this.expandedTimeGroups.add(time);
        }
        
        return {
          timeLabel: time,
          medications: medications,
          allTaken: allTaken
        };
      });
    
    // Sort: missed groups first
    this.groupedMedications.sort((a, b) => {
      if (!a.allTaken && b.allTaken) return -1;
      if (a.allTaken && !b.allTaken) return 1;
      return 0;
    });
    
    // Auto-start carousel at first missed medication group
    const firstMissedIndex = this.groupedMedications.findIndex(g => !g.allTaken);
    this.currentTimeIndex = firstMissedIndex >= 0 ? firstMissedIndex : 0;
  }

  toggleTimeGroup(timeLabel: string) {
    if (this.expandedTimeGroups.has(timeLabel)) {
      this.expandedTimeGroups.delete(timeLabel);
    } else {
      this.expandedTimeGroups.add(timeLabel);
    }
  }

  isTimeGroupExpanded(timeLabel: string): boolean {
    return this.expandedTimeGroups.has(timeLabel);
  }
  
  // Mobile Carousel Methods
  nextTimeGroup() {
    if (this.currentTimeIndex < this.groupedMedications.length - 1) {
      this.currentTimeIndex++;
    }
  }
  
  prevTimeGroup() {
    if (this.currentTimeIndex > 0) {
      this.currentTimeIndex--;
    }
  }
  
  goToTimeGroup(index: number) {
    this.currentTimeIndex = index;
  }
  
  onTouchStart(event: TouchEvent) {
    this.touchStartX = event.touches[0].screenX;
  }
  
  onTouchEnd(event: TouchEvent) {
    this.touchEndX = event.changedTouches[0].screenX;
    this.handleSwipe();
  }
  
  handleSwipe() {
    const swipeThreshold = 50;
    const diff = this.touchStartX - this.touchEndX;
    
    if (Math.abs(diff) > swipeThreshold) {
      if (diff > 0) {
        // Swipe left -> next
        this.nextTimeGroup();
      } else {
        // Swipe right -> prev
        this.prevTimeGroup();
      }
    }
  }

  getFormattedDate(date: string): string {
    return new Date(date).toLocaleDateString('de-DE', { 
      weekday: 'long', 
      day: '2-digit', 
      month: 'long', 
      year: 'numeric' 
    });
  }

  exportMedicationPlan() {
    const doc = new jsPDF();
    
    // Titel
    doc.setFontSize(16);
    doc.text('Medikamentenplan', 14, 20);
    
    // Untertitel mit aktuellem Monat
    doc.setFontSize(12);
    doc.text(this.currentMonth, 14, 28);
    
    // Tabellendaten vorbereiten
    const headers = [['Zeit', 'Mo', 'Di', 'Mi', 'Do', 'Fr', 'Sa', 'So']];
    const data = this.medicationRows.map(row => [
      row.timeLabel,
      ...row.days
    ]);
    
    // Tabelle erstellen
    autoTable(doc, {
      head: headers,
      body: data,
      startY: 35,
      styles: {
        fontSize: 10,
        cellPadding: 3,
        halign: 'center'
      },
      headStyles: {
        fillColor: [41, 128, 185],
        textColor: 255,
        fontStyle: 'bold'
      },
      columnStyles: {
        0: { halign: 'left', cellWidth: 30 }
      }
    });
    
    // PDF speichern
    const fileName = `Medikamentenplan_${this.currentMonth.replace(' ', '_')}.pdf`;
    doc.save(fileName);
  }

  // Wizard Methods
  openCreateWizard() {
    this.showCreateWizard = true;
    this.showPlanSelection = false;
    this.currentWizardStep = 0;
    this.resetNewPlan();
  }

  closeCreateWizard() {
    this.showCreateWizard = false;
    this.resetNewPlan();
  }

  resetNewPlan() {
    this.weekSchedule = new Map();
    // Initialisiere 7 Tage (Mo-So)
    for (let dayIndex = 0; dayIndex < 7; dayIndex++) {
      const dayMap = new Map<string, Array<any>>();
      // Initialisiere 4 Tageszeiten pro Tag
      this.timesOfDay.forEach(time => {
        dayMap.set(time, []);
      });
      this.weekSchedule.set(dayIndex, dayMap);
    }
    // Alle Tageszeit-Formulare zurücksetzen
    this.resetCurrentMedication();
  }

  nextWizardStep() {
    if (this.currentWizardStep < this.wizardSteps.length - 1) {
      this.currentWizardStep++;
    }
  }

  previousWizardStep() {
    if (this.currentWizardStep > 0) {
      this.currentWizardStep--;
    }
  }

  getCurrentDayIndex(): number {
    // Schritt 0-6 = Mo-So, Schritt 7 = Zusammenfassung
    return this.currentWizardStep;
  }

  hasMedicationsForDay(dayIndex: number): boolean {
    const dayMap = this.weekSchedule.get(dayIndex);
    if (!dayMap) return false;
    
    let hasMeds = false;
    dayMap.forEach(medications => {
      if (medications.length > 0) hasMeds = true;
    });
    return hasMeds;
  }

  isCurrentStepValid(): boolean {
    if (this.currentWizardStep >= 0 && this.currentWizardStep <= 6) {
      // Wochentage: Optional, kann übersprungen werden
      return true;
    }
    if (this.currentWizardStep === 7) {
      // Zusammenfassung: Mind. 1 Medikament muss irgendwo hinzugefügt sein
      let totalMeds = 0;
      this.weekSchedule.forEach(dayMap => {
        dayMap.forEach(medications => {
          totalMeds += medications.length;
        });
      });
      return totalMeds > 0;
    }
    return false;
  }

  getPatientName(patientId: number | null): string {
    return this.userName;
  }

  addMedicationToTimeSlot(dayIndex: number, timeOfDay: string) {
    const current = this.currentMedications[timeOfDay];
    if ((current.medicationId || current.name.trim()) && current.dosage > 0) {
      const dayMap = this.weekSchedule.get(dayIndex);
      if (!dayMap) return;
      
      const medications = dayMap.get(timeOfDay) || [];
      medications.push({
        medicationId: current.medicationId,
        name: current.name,
        dosage: current.dosage,
        dosageUnit: current.dosageUnit
      });
      
      dayMap.set(timeOfDay, medications);
      
      // Formular für diese Tageszeit zurücksetzen
      this.resetCurrentMedication(timeOfDay);
    }
  }

  // Medikament für andere Tage zur gleichen Tageszeit wiederholen
  repeatMedicationForOtherDays(sourceDayIndex: number, timeOfDay: string, medIndex: number) {
    const sourceDayMap = this.weekSchedule.get(sourceDayIndex);
    if (!sourceDayMap) return;
    
    const medications = sourceDayMap.get(timeOfDay);
    if (!medications || !medications[medIndex]) return;
    
    const medToRepeat = medications[medIndex];
    
    // Frage welche Tage
    const dayNames = ['Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag', 'Sonntag'];
    const selectedDays: number[] = [];
    
    // Zeige Auswahl-Dialog (vereinfacht: alle anderen Tage)
    for (let i = 0; i < 7; i++) {
      if (i !== sourceDayIndex) {
        const targetDayMap = this.weekSchedule.get(i);
        if (targetDayMap) {
          const targetMedications = targetDayMap.get(timeOfDay) || [];
          targetMedications.push({
            medicationId: medToRepeat.medicationId,
            name: medToRepeat.name,
            dosage: medToRepeat.dosage,
            dosageUnit: medToRepeat.dosageUnit
          });
          targetDayMap.set(timeOfDay, targetMedications);
        }
      }
    }
    
    alert(`${medToRepeat.name} wurde für alle anderen Tage zur gleichen Tageszeit (${this.timeLabels[timeOfDay]}) übernommen!`);
  }

  resetCurrentMedication(timeOfDay?: string) {
    if (timeOfDay) {
      // Nur für eine bestimmte Tageszeit zurücksetzen
      this.currentMedications[timeOfDay] = {
        medicationId: null,
        name: '',
        dosage: 1,
        dosageUnit: 'Tablette(n)'
      };
    } else {
      // Alle Tageszeiten zurücksetzen
      this.timesOfDay.forEach(time => {
        this.currentMedications[time] = {
          medicationId: null,
          name: '',
          dosage: 1,
          dosageUnit: 'Tablette(n)'
        };
      });
    }
  }

  removeMedicationFromTimeSlot(dayIndex: number, timeOfDay: string, medIndex: number) {
    const dayMap = this.weekSchedule.get(dayIndex);
    if (!dayMap) return;
    
    const medications = dayMap.get(timeOfDay) || [];
    medications.splice(medIndex, 1);
    dayMap.set(timeOfDay, medications);
  }

  getMedicationsForTimeSlot(dayIndex: number, timeOfDay: string): Array<any> {
    const dayMap = this.weekSchedule.get(dayIndex);
    if (!dayMap) return [];
    return dayMap.get(timeOfDay) || [];
  }

  onMedicationSelect(timeOfDay: string) {
    const current = this.currentMedications[timeOfDay];
    const selected = this.availableMedications.find(m => m.id === current.medicationId);
    if (selected) {
      current.name = selected.name;
    } else {
      // Wenn "neu eingeben" gewählt wurde, Name leeren
      current.name = '';
    }
  }

  getScheduleSummary(): string {
    const dayNames = ['Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag', 'Sonntag'];
    
    let summary = '';
    let hasAnyMedication = false;
    
    this.weekSchedule.forEach((dayMap, dayIndex) => {
      let dayHasMeds = false;
      let dayContent = `\n${dayNames[dayIndex]}:\n`;
      
      dayMap.forEach((medications, timeOfDay) => {
        if (medications.length > 0) {
          dayHasMeds = true;
          hasAnyMedication = true;
          dayContent += `  ${this.timeLabels[timeOfDay]}:\n`;
          medications.forEach(med => {
            dayContent += `    • ${med.name} (${med.dosage} ${med.dosageUnit})\n`;
          });
        }
      });
      
      if (dayHasMeds) {
        summary += dayContent;
      }
    });
    
    return hasAnyMedication ? summary : 'Keine Medikamente hinzugefügt';
  }

  createMedicationPlan() {
    // Zähle Gesamtanzahl Medikamente
    let totalMeds = 0;
    this.weekSchedule.forEach(dayMap => {
      dayMap.forEach(medications => {
        totalMeds += medications.length;
      });
    });

    if (totalMeds === 0) {
      alert('Bitte fügen Sie mindestens ein Medikament hinzu!');
      return;
    }

    console.log('Erstelle Medikamentenplan mit', totalMeds, 'Einnahmen...');

    this.medicationPlanService.createWeeklyMedicationPlans(this.weekSchedule, this.currentPatientId).subscribe({
      next: (createdPlans) => {
        console.log('Erfolgreich erstellt:', createdPlans);
        alert(`✓ Medikamentenplan erfolgreich erstellt!\n${createdPlans.length} Einträge gespeichert.`);
        this.closeCreateWizard();
        this.loadMedicationPlans();
      },
      error: (error) => {
        console.error('Fehler beim Erstellen des Plans:', error);
        console.error('Error Details:', error.error);
        if (error.error?.errors) {
          console.error('Validation Errors:', error.error.errors);
        }
        
        let errorMsg = '⚠ Fehler beim Speichern des Medikamentenplans!\n\n';
        if (error.error?.errors) {
          Object.keys(error.error.errors).forEach(key => {
            errorMsg += `${key}: ${error.error.errors[key].join(', ')}\n`;
          });
        }
        alert(errorMsg);
      }
    });
  }
}
