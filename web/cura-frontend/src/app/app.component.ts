import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
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
  
  calendarDays: CalendarDay[] = [];
  medicationPlans: MedicationPlan[] = [];
  medicationRows: { timeLabel: string; days: string[] }[] = [];
  intakeQuote = 85;

  constructor(
    private calendarService: CalendarService,
    private medicationPlanService: MedicationPlanService
  ) {}

  ngOnInit() {
    this.currentMonth = this.calendarService.getCurrentMonth();
    this.calendarDays = this.calendarService.generateCalendar();
    this.loadMedicationPlans();
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
}
