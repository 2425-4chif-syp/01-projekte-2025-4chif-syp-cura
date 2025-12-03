import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { KeycloakService } from 'keycloak-angular';
import { CalendarDay } from './models/calendar-day.model';
import { MedicationPlan } from './models/medication-plan.model';
import { CalendarService } from './services/calendar.service';
import { MedicationPlanService } from './services/medication-plan.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule],
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
    this.loadMedicationPlans();
  }

  logout() {
    this.keycloak.logout(window.location.origin);
  }

  loadCalendar() {
    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth() + 1;

    this.calendarService.getDailyStatus(1, year, month).subscribe({
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
    this.medicationPlanService.getMedicationPlans(1).subscribe({
      next: (plans) => {
        this.medicationPlans = plans;
        this.loadMedicationNames(plans);
      },
      error: () => {
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
    // Lade echte Einnahme-Daten vom Backend
    this.medicationPlanService.getMedicationIntakesForDate(1, date).subscribe({
      next: (intakes) => {
        this.selectedDayMedications = intakes.map(intake => ({
          timeLabel: intake.timeLabel,
          medication: intake.medicationName,
          status: intake.isTaken ? 'taken' : 'missed'
        }));
      },
      error: (err) => {
        console.error('Error loading day medications:', err);
        // Fallback: Zeige leere Liste
        this.selectedDayMedications = [];
      }
    });
  }

  getFormattedDate(date: string): string {
    return new Date(date).toLocaleDateString('de-DE', { 
      weekday: 'long', 
      day: '2-digit', 
      month: 'long', 
      year: 'numeric' 
    });
  }
}
