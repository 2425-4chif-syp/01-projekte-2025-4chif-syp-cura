import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MedicationService } from '../../services/medication.service';
import { MedicationPlan, Drug, WeekDay } from '../../models/drug.interface';
import { PlanDialogComponent } from '../plan-dialog/plan-dialog.component';

@Component({
  selector: 'app-medication-plan',
  template: `
    <div class="container mt-4">
      <h2>Medikamentenplan</h2>

      <!-- Fehlermeldungen -->
      <div *ngIf="errorMessage" class="alert alert-danger alert-dismissible fade show" role="alert">
        {{errorMessage}}
        <button type="button" class="btn-close" (click)="errorMessage = ''"></button>
      </div>

      <div *ngIf="successMessage" class="alert alert-success alert-dismissible fade show" role="alert">
        {{successMessage}}
        <button type="button" class="btn-close" (click)="successMessage = ''"></button>
      </div>

      <!-- Suchleiste -->
      <div class="row mb-3">
        <div class="col-md-6">
          <div class="input-group">
            <input 
              type="text" 
              class="form-control form-control-lg" 
              placeholder="Suche nach Medikamenten oder Tagen..." 
              [(ngModel)]="searchTerm"
              (input)="filterPlan()"
            >
            <select class="form-select form-select-lg" style="max-width: 200px" [(ngModel)]="filterField" (change)="filterPlan()">
              <option value="drug">Medikament</option>
              <option value="day">Tag</option>
              <option value="time">Uhrzeit</option>
              <option value="all">Alle Felder</option>
            </select>
          </div>
        </div>
        <div class="col-md-6 text-end">
          <button class="btn btn-primary btn-lg" (click)="openAddDialog()">
            <i class="bi bi-plus-circle me-2"></i>Neue Einnahme planen
          </button>
        </div>
      </div>

      <!-- Tagesweise Ansicht -->
      <div class="day-cards-container">
        <ng-container *ngFor="let dayGroup of groupedPlan">
          <div [class.sunday-container]="dayGroup.day === 'Sunday'" 
               [style.display]="dayGroup.day === 'Sunday' ? 'none' : 'block'">
            <div class="card day-card">
              <div class="card-header bg-primary text-white">
                <div class="d-flex justify-content-between align-items-center">
                  <h3 class="mb-0">{{getDayName(dayGroup.day)}}</h3>
                  <button 
                    class="btn btn-collapse" 
                    (click)="toggleDay(dayGroup.day)"
                    [attr.aria-expanded]="!collapsedDays.includes(dayGroup.day)"
                  >
                    <i class="bi" [class.bi-chevron-up]="!collapsedDays.includes(dayGroup.day)" [class.bi-chevron-down]="collapsedDays.includes(dayGroup.day)"></i>
                  </button>
                </div>
              </div>
              <div class="card-body" [class.collapsed]="collapsedDays.includes(dayGroup.day)">
                <div *ngFor="let timeGroup of groupEntriesByTime(dayGroup.entries)" class="mb-4">
                  <h4 class="time-header">{{timeGroup.time}} Uhr</h4>
                  <div *ngFor="let entry of timeGroup.entries" class="medication-entry p-4 mb-3">
                    <div class="medication-content">
                      <div class="medication-info">
                        <h3 class="medication-name mb-2">{{entry.drug?.name}}</h3>
                        <p class="medication-description">{{entry.drug?.description}}</p>
                      </div>
                      <div class="medication-actions">
                        <button 
                          class="btn btn-action btn-edit mb-3" 
                          (click)="editPlanEntry(entry)"
                          title="Bearbeiten"
                        >
                          <i class="bi bi-pencil-square"></i>
                          <span class="action-text">Bearbeiten</span>
                        </button>
                        <button 
                          class="btn btn-action btn-delete" 
                          (click)="deletePlanEntry(entry.id)"
                          title="Löschen"
                        >
                          <i class="bi bi-trash"></i>
                          <span class="action-text">Löschen</span>
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
                <div *ngIf="dayGroup.entries.length === 0" class="text-center text-muted">
                  Keine Einnahmen geplant
                </div>
              </div>
            </div>
          </div>
        </ng-container>
      </div>

      <!-- Sonntag separat -->
      <div class="day-cards-container">
        <ng-container *ngFor="let dayGroup of groupedPlan">
          <div *ngIf="dayGroup.day === 'Sunday'" class="sunday-container">
            <div class="card day-card">
              <div class="card-header bg-primary text-white">
                <div class="d-flex justify-content-between align-items-center">
                  <h3 class="mb-0">{{getDayName(dayGroup.day)}}</h3>
                  <button 
                    class="btn btn-collapse" 
                    (click)="toggleDay(dayGroup.day)"
                    [attr.aria-expanded]="!collapsedDays.includes(dayGroup.day)"
                  >
                    <i class="bi" [class.bi-chevron-up]="!collapsedDays.includes(dayGroup.day)" [class.bi-chevron-down]="collapsedDays.includes(dayGroup.day)"></i>
                  </button>
                </div>
              </div>
              <div class="card-body" [class.collapsed]="collapsedDays.includes(dayGroup.day)">
                <div *ngFor="let timeGroup of groupEntriesByTime(dayGroup.entries)" class="mb-4">
                  <h4 class="time-header">{{timeGroup.time}} Uhr</h4>
                  <div *ngFor="let entry of timeGroup.entries" class="medication-entry p-4 mb-3">
                    <div class="medication-content">
                      <div class="medication-info">
                        <h3 class="medication-name mb-2">{{entry.drug?.name}}</h3>
                        <p class="medication-description">{{entry.drug?.description}}</p>
                      </div>
                      <div class="medication-actions">
                        <button 
                          class="btn btn-action btn-edit mb-3" 
                          (click)="editPlanEntry(entry)"
                          title="Bearbeiten"
                        >
                          <i class="bi bi-pencil-square"></i>
                          <span class="action-text">Bearbeiten</span>
                        </button>
                        <button 
                          class="btn btn-action btn-delete" 
                          (click)="deletePlanEntry(entry.id)"
                          title="Löschen"
                        >
                          <i class="bi bi-trash"></i>
                          <span class="action-text">Löschen</span>
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
                <div *ngIf="dayGroup.entries.length === 0" class="text-center text-muted">
                  Keine Einnahmen geplant
                </div>
              </div>
            </div>
          </div>
        </ng-container>
      </div>
    </div>

    <!-- Dialog -->
    <app-plan-dialog
      *ngIf="showDialog"
      [planEntry]="selectedEntry"
      [availableDrugs]="availableDrugs"
      (save$)="savePlanEntry($event)"
      (cancel$)="closeDialog()"
    ></app-plan-dialog>
  `,
  styles: [`
    .container { 
      max-width: 1400px;
      min-height: 100vh;
      padding-bottom: 6rem;
    }
    .day-cards-container {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 1.5rem;
      margin-bottom: 1.5rem;
    }
    .day-card {
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      transition: box-shadow 0.3s ease;
      height: 100%;
    }
    .sunday-container {
      grid-column: 2;
      margin-top: 1rem;
    }
    .day-card:hover {
      box-shadow: 0 4px 8px rgba(0,0,0,0.2);
    }
    .card-header {
      padding: 1rem;
    }
    .time-header {
      font-size: 1.4rem;
      color: #2c3e50;
      border-bottom: 2px solid #e9ecef;
      padding-bottom: 0.8rem;
      margin-bottom: 1.2rem;
      font-weight: bold;
    }
    .medication-entry {
      background-color: white;
      border-radius: 10px;
      transition: all 0.3s ease;
      border: 1px solid #e9ecef;
      box-shadow: 0 2px 4px rgba(0,0,0,0.05);
      padding: 1.2rem;
      margin-bottom: 1rem;
    }
    .medication-entry:hover {
      background-color: #f8f9fa;
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0,0,0,0.1);
    }
    .medication-content {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      gap: 1.5rem;
    }
    .medication-info {
      flex-grow: 1;
    }
    .medication-name {
      font-size: 1.5rem;
      color: #2c3e50;
      font-weight: bold;
      margin-bottom: 0.4rem;
    }
    .medication-description {
      font-size: 1.1rem;
      color: #6c757d;
      margin-bottom: 0;
      line-height: 1.4;
    }
    .medication-actions {
      display: flex;
      flex-direction: column;
      gap: 0.8rem;
      min-width: 140px;
    }
    .btn-action {
      padding: 0.7rem 1rem;
      border-radius: 8px;
      transition: all 0.3s ease;
      width: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.8rem;
      font-size: 1.1rem;
      font-weight: 500;
    }
    .btn-action i {
      font-size: 1.3rem;
    }
    .action-text {
      display: inline-block;
    }
    .btn-edit {
      background-color: #0d6efd;
      color: white;
      border: none;
    }
    .btn-edit:hover {
      background-color: #0b5ed7;
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(13, 110, 253, 0.3);
    }
    .btn-delete {
      background-color: #dc3545;
      color: white;
      border: none;
    }
    .btn-delete:hover {
      background-color: #bb2d3b;
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(220, 53, 69, 0.3);
    }
    .btn-collapse {
      background: transparent;
      border: none;
      color: white;
      padding: 0.5rem;
      font-size: 1.5rem;
      line-height: 1;
      transition: transform 0.3s ease;
    }
    .btn-collapse:hover {
      color: rgba(255, 255, 255, 0.8);
    }
    .card-body {
      transition: max-height 0.3s ease-out, opacity 0.3s ease-out;
      max-height: 1000px;
      opacity: 1;
      overflow: hidden;
    }
    .card-body.collapsed {
      max-height: 0;
      opacity: 0;
      padding-top: 0;
      padding-bottom: 0;
    }
  `],
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule, PlanDialogComponent]
})
export class MedicationPlanComponent implements OnInit {
  medicationPlan: MedicationPlan[] = [];
  filteredPlan: MedicationPlan[] = [];
  availableDrugs: Drug[] = [];
  errorMessage: string = '';
  successMessage: string = '';
  showDialog = false;
  selectedEntry?: MedicationPlan;
  searchTerm: string = '';
  filterField: 'drug' | 'day' | 'time' | 'all' = 'drug';
  collapsedDays: WeekDay[] = [];

  constructor(private medicationService: MedicationService) {}

  ngOnInit() {
    this.loadMedicationPlan();
    this.loadDrugs();
  }

  loadMedicationPlan() {
    this.medicationService.getMedicationPlan().subscribe(
      (plan) => {
        console.log('Geladener Plan:', plan); // Debug-Ausgabe
        this.medicationPlan = plan;
        this.filterPlan();
      },
      (error) => {
        console.error('Fehler beim Laden des Medikamentenplans:', error);
        this.errorMessage = 'Fehler beim Laden des Medikamentenplans: ' + (error.error?.error || error.message);
      }
    );
  }

  loadDrugs() {
    this.medicationService.getDrugs().subscribe(
      (drugs) => {
        this.availableDrugs = drugs;
      },
      (error) => {
        console.error('Fehler beim Laden der Medikamente:', error);
        this.errorMessage = 'Fehler beim Laden der Medikamente: ' + (error.error?.error || error.message);
      }
    );
  }

  filterPlan() {
    if (!this.searchTerm.trim()) {
      this.filteredPlan = this.medicationPlan;
      return;
    }

    const searchTermLower = this.searchTerm.toLowerCase().trim();
    
    this.filteredPlan = this.medicationPlan.filter(entry => {
      const dayName = this.getDayName(entry.day).toLowerCase();
      
      switch (this.filterField) {
        case 'drug':
          return entry.drug?.name.toLowerCase().includes(searchTermLower);
        case 'day':
          return dayName.includes(searchTermLower);
        case 'time':
          return entry.time.includes(searchTermLower);
        case 'all':
          return (
            entry.drug?.name.toLowerCase().includes(searchTermLower) ||
            dayName.includes(searchTermLower) ||
            entry.time.includes(searchTermLower)
          );
      }
    });
  }

  getDayName(day: string): string {
    const dayMap: { [key: string]: string } = {
      'Monday': 'Montag',
      'Tuesday': 'Dienstag',
      'Wednesday': 'Mittwoch',
      'Thursday': 'Donnerstag',
      'Friday': 'Freitag',
      'Saturday': 'Samstag',
      'Sunday': 'Sonntag'
    };
    return dayMap[day] || day;
  }

  openAddDialog() {
    this.selectedEntry = undefined;
    this.showDialog = true;
  }

  editPlanEntry(entry: MedicationPlan) {
    this.selectedEntry = entry;
    this.showDialog = true;
  }

  closeDialog() {
    this.showDialog = false;
    this.selectedEntry = undefined;
  }

  savePlanEntry(entries: MedicationPlan[]) {
    if (entries.length === 0) return;

    let successCount = 0;
    let errorCount = 0;
    let duplicateCount = 0;

    entries.forEach(entry => {
      if (entry.id) {
        // Bearbeiten
        this.medicationService.updatePlanEntry(entry.id, entry).subscribe(
          () => {
            successCount++;
            this.checkCompletion(successCount, errorCount, duplicateCount, entries.length);
          },
          (error) => {
            console.error('Fehler beim Aktualisieren der Einnahme:', error);
            if (error.status === 400) {
              duplicateCount++;
            } else {
              errorCount++;
            }
            this.checkCompletion(successCount, errorCount, duplicateCount, entries.length);
          }
        );
      } else {
        // Neu hinzufügen
        this.medicationService.addToPlan(entry).subscribe(
          () => {
            successCount++;
            this.checkCompletion(successCount, errorCount, duplicateCount, entries.length);
          },
          (error) => {
            console.error('Fehler beim Planen der Einnahme:', error);
            if (error.status === 400) {
              duplicateCount++;
            } else {
              errorCount++;
            }
            this.checkCompletion(successCount, errorCount, duplicateCount, entries.length);
          }
        );
      }
    });
  }

  private checkCompletion(successCount: number, errorCount: number, duplicateCount: number, totalCount: number) {
    if (successCount + errorCount + duplicateCount === totalCount) {
      if (errorCount === 0 && duplicateCount === 0) {
        this.successMessage = 'Alle Einnahmen erfolgreich gespeichert';
      } else if (successCount === 0 && duplicateCount === 0) {
        this.errorMessage = 'Fehler beim Speichern der Einnahmen';
      } else if (duplicateCount > 0) {
        if (successCount > 0) {
          this.successMessage = `${successCount} neue Einnahmen gespeichert. ${duplicateCount} Einnahmen existieren bereits.`;
        } else {
          this.errorMessage = `Diese Einnahmen existieren bereits im Plan.`;
        }
      } else {
        this.successMessage = `${successCount} Einnahmen gespeichert, ${errorCount} fehlgeschlagen`;
      }
      
      this.loadMedicationPlan();
      this.closeDialog();
      
      if (this.successMessage) {
        setTimeout(() => this.successMessage = '', 5000); // Zeige die Nachricht etwas länger an
      }
    }
  }

  deletePlanEntry(id: number | undefined) {
    if (!id) return;
    
    if (confirm('Möchten Sie diesen Eintrag wirklich löschen?')) {
      this.medicationService.deletePlanEntry(id).subscribe(
        (response) => {
          this.successMessage = 'Eintrag erfolgreich gelöscht';
          this.loadMedicationPlan();
          setTimeout(() => this.successMessage = '', 3000);
        },
        (error) => {
          console.error('Fehler beim Löschen des Eintrags:', error);
          this.errorMessage = error.error?.error || 'Fehler beim Löschen des Eintrags';
        }
      );
    }
  }

  get groupedPlan() {
    const days: WeekDay[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
    return days.map(day => ({
      day,
      entries: this.filteredPlan.filter(entry => entry.day === day)
    }));
  }

  groupEntriesByTime(entries: MedicationPlan[]) {
    const timeGroups = entries.reduce((groups, entry) => {
      const time = entry.time;
      if (!groups[time]) {
        groups[time] = [];
      }
      groups[time].push(entry);
      return groups;
    }, {} as { [key: string]: MedicationPlan[] });

    return Object.entries(timeGroups)
      .sort(([timeA], [timeB]) => timeA.localeCompare(timeB))
      .map(([time, entries]) => ({ time, entries }));
  }

  toggleDay(day: WeekDay) {
    const index = this.collapsedDays.indexOf(day);
    if (index === -1) {
      this.collapsedDays.push(day);
    } else {
      this.collapsedDays.splice(index, 1);
    }
  }
} 