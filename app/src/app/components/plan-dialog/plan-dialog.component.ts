import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MedicationPlan, Drug, WeekDay } from '../../models/drug.interface';

@Component({
  selector: 'app-plan-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal fade show" style="display: block; background-color: rgba(0,0,0,0.5)">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ planEntry?.id ? 'Einnahme bearbeiten' : 'Neue Einnahme planen' }}</h5>
            <button type="button" class="btn-close" (click)="cancel()"></button>
          </div>
          <div class="modal-body">
            <form #planForm="ngForm">
              <!-- Medikament -->
              <div class="mb-4">
                <label for="drug" class="form-label fs-5">Welches Medikament möchten Sie einnehmen?</label>
                <select 
                  class="form-select form-select-lg"
                  id="drug"
                  name="drug_id"
                  [(ngModel)]="editedPlan.drug_id"
                  required
                >
                  <option [ngValue]="undefined" disabled>Bitte wählen Sie ein Medikament...</option>
                  <option *ngFor="let drug of availableDrugs" [value]="drug.id">
                    {{drug.name}}
                  </option>
                </select>
              </div>

              <!-- Uhrzeit -->
              <div class="mb-4">
                <label class="form-label fs-5">Zu welcher Uhrzeit möchten Sie das Medikament einnehmen?</label>
                <div class="d-flex flex-wrap gap-2">
                  <button 
                    *ngFor="let time of availableTimes" 
                    type="button"
                    class="btn btn-outline-primary btn-lg"
                    [class.active]="editedPlan.time === time"
                    (click)="editedPlan.time = time"
                  >
                    {{time}} Uhr
                  </button>
                </div>
              </div>

              <!-- Wochentage -->
              <div class="mb-4">
                <label class="form-label fs-5">An welchen Tagen möchten Sie das Medikament einnehmen?</label>
                <div class="d-flex flex-wrap gap-3 mt-3">
                  <button 
                    *ngFor="let day of availableDays"
                    type="button"
                    class="btn btn-outline-primary btn-xl day-button"
                    [class.active]="selectedDays.includes(day.value)"
                    (click)="toggleDay(day.value)"
                  >
                    {{day.label}}
                  </button>
                </div>
                <div class="mt-2">
                  <button type="button" class="btn btn-outline-secondary" (click)="selectAllDays()">
                    Alle Tage auswählen
                  </button>
                </div>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary btn-lg" (click)="cancel()">Abbrechen</button>
            <button 
              type="button" 
              class="btn btn-primary btn-lg" 
              (click)="save()"
              [disabled]="!isValid()"
            >
              Speichern
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .day-button {
      font-size: 1.5rem;
      padding: 1rem 2rem;
      min-width: 200px;
      text-align: center;
    }
    .btn-xl {
      padding: 1rem 2rem;
      font-size: 1.5rem;
      border-radius: 0.5rem;
    }
  `]
})
export class PlanDialogComponent {
  @Input() planEntry?: MedicationPlan;
  @Input() availableDrugs: Drug[] = [];
  @Output() save$ = new EventEmitter<MedicationPlan[]>();
  @Output() cancel$ = new EventEmitter<void>();

  editedPlan: Omit<MedicationPlan, 'day'> & { day?: WeekDay } = {
    drug_id: undefined as any,
    time: ''
  };

  selectedDays: WeekDay[] = [];

  availableDays: { value: WeekDay; label: string }[] = [
    { value: 'Monday', label: 'Montag' },
    { value: 'Tuesday', label: 'Dienstag' },
    { value: 'Wednesday', label: 'Mittwoch' },
    { value: 'Thursday', label: 'Donnerstag' },
    { value: 'Friday', label: 'Freitag' },
    { value: 'Saturday', label: 'Samstag' },
    { value: 'Sunday', label: 'Sonntag' }
  ];

  availableTimes = ['08:00', '12:00', '16:00', '20:00'];

  ngOnInit() {
    if (this.planEntry) {
      this.editedPlan = { ...this.planEntry };
      this.selectedDays = [this.planEntry.day];
    }
  }

  toggleDay(day: WeekDay) {
    const index = this.selectedDays.indexOf(day);
    if (index === -1) {
      this.selectedDays.push(day);
    } else {
      this.selectedDays.splice(index, 1);
    }
  }

  selectAllDays() {
    this.selectedDays = this.availableDays.map(d => d.value);
  }

  isValid(): boolean {
    return !!(this.editedPlan.drug_id && this.selectedDays.length > 0 && this.editedPlan.time);
  }

  save() {
    if (this.isValid()) {
      const entries: MedicationPlan[] = this.selectedDays.map(day => ({
        ...this.editedPlan,
        day,
        id: this.planEntry?.day === day ? this.planEntry.id : undefined
      }));
      this.save$.emit(entries);
    }
  }

  cancel() {
    this.cancel$.emit();
  }
} 