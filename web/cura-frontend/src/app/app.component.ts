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
  selectedPlanId = 1;
  availablePlans: { id: number; name: string; patientName: string }[] = [];
  groupedMedications: { timeLabel: string; medications: { name: string; status: 'taken' | 'missed' }[]; allTaken: boolean }[] = [];
  expandedTimeGroups = new Set<string>();
  
  // Mobile Carousel
  currentTimeIndex = 0;
  touchStartX = 0;
  touchEndX = 0;

  // Create Plan Wizard
  showCreateWizard = false;
  currentWizardStep = 0;
  wizardSteps = ['Patient', 'Medikament', 'Tageszeiten', 'Wochentage', 'Zusammenfassung'];
  
  newPlan = {
    patientId: null as number | null,
    medicationName: '',
    dosage: 1,
    dosageUnit: 'Tablette(n)',
    timesOfDay: [] as string[],
    weekdays: [] as number[]
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
    } catch (error) {
      console.error('Fehler beim Laden des User-Profils:', error);
    }

    // Rest des Codes
    this.currentMonth = this.calendarService.getCurrentMonth();
    this.loadCalendar();
    this.loadAvailablePlans();
    this.loadMedicationPlans();
  }

  logout() {
    this.keycloak.logout(window.location.origin);
  }

  // Medication Plan Selection
  loadAvailablePlans() {
    this.medicationPlanService.getAllAvailablePlans().subscribe({
      next: (plans) => {
        this.availablePlans = plans;
        // Setze ersten Plan als Standard, falls vorhanden
        if (plans.length > 0 && !this.selectedPlanId) {
          this.selectedPlanId = plans[0].id;
          this.loadMedicationPlans();
        }
      },
      error: (error) => {
        console.error('Fehler beim Laden der verfügbaren Pläne:', error);
        // Fallback zu Default-Wert
        this.availablePlans = [
          { id: 1, name: 'Medikamentenplan 1', patientName: 'Standardpatient' }
        ];
      }
    });
  }

  openPlanSelection() {
    this.showPlanSelection = true;
  }

  closePlanSelection() {
    this.showPlanSelection = false;
  }

  selectPlan(planId: number) {
    this.selectedPlanId = planId;
    this.closePlanSelection();
    // Lade Medikamentenpläne für den ausgewählten Patienten neu
    this.loadMedicationPlans();
    this.loadCalendar();
  }

  get selectedPlanName(): string {
    const plan = this.availablePlans.find(p => p.id === this.selectedPlanId);
    return plan?.name || 'Medikamentenplan';
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
    // Verwende selectedPlanId (= patientId) statt hardcoded 1
    const patientId = this.selectedPlanId || 1;
    
    this.medicationPlanService.getMedicationPlans(patientId).subscribe({
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
  openCreatePlanWizard() {
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
    this.newPlan = {
      patientId: null,
      medicationName: '',
      dosage: 1,
      dosageUnit: 'Tablette(n)',
      timesOfDay: [],
      weekdays: []
    };
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

  toggleTimeOfDay(time: string) {
    const index = this.newPlan.timesOfDay.indexOf(time);
    if (index > -1) {
      this.newPlan.timesOfDay.splice(index, 1);
    } else {
      this.newPlan.timesOfDay.push(time);
    }
  }

  toggleWeekday(day: number) {
    const index = this.newPlan.weekdays.indexOf(day);
    if (index > -1) {
      this.newPlan.weekdays.splice(index, 1);
    } else {
      this.newPlan.weekdays.push(day);
    }
  }

  isCurrentStepValid(): boolean {
    switch (this.currentWizardStep) {
      case 0: // Patient
        return this.newPlan.patientId !== null;
      case 1: // Medikament
        return this.newPlan.medicationName.trim() !== '' && this.newPlan.dosage > 0;
      case 2: // Tageszeiten
        return this.newPlan.timesOfDay.length > 0;
      case 3: // Wochentage
        return this.newPlan.weekdays.length > 0;
      case 4: // Zusammenfassung
        return true;
      default:
        return false;
    }
  }

  getPatientName(patientId: number | null): string {
    if (!patientId) return '-';
    const plan = this.availablePlans.find(p => p.id === patientId);
    return plan?.patientName || '-';
  }

  getTimeLabels(): string {
    const labels = {
      'MORNING': 'Morgen',
      'NOON': 'Mittag',
      'AFTERNOON': 'Nachmittag',
      'EVENING': 'Abend'
    };
    return this.newPlan.timesOfDay.map(t => labels[t as keyof typeof labels]).join(', ') || '-';
  }

  getWeekdayLabels(): string {
    const selectedDays = this.weekdays.filter(d => this.newPlan.weekdays.includes(d.value));
    return selectedDays.map(d => d.short).join(', ') || '-';
  }

  createMedicationPlan() {
    console.log('Erstelle Medikamentenplan:', this.newPlan);
    
    // TODO: API-Call zum Backend um Plan zu erstellen
    // this.medicationPlanService.createPlan(this.newPlan).subscribe({...});
    
    // Vorerst nur schließen und Bestätigung zeigen
    alert('Plan erstellt! (Backend-Integration folgt)');
    this.closeCreateWizard();
    this.loadAvailablePlans();
  }
}
