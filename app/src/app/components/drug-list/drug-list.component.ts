import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MedicationService } from '../../services/medication.service';
import { Drug } from '../../models/drug.interface';
import { DrugDialogComponent } from '../drug-dialog/drug-dialog.component';

@Component({
  selector: 'app-drug-list',
  template: `
    <div class="container mt-4">
      <h2 class="mb-4">Medikamentenliste</h2>
      
      <!-- Fehlermeldung -->
      <div *ngIf="errorMessage" class="alert alert-danger alert-dismissible fade show" role="alert">
        {{errorMessage}}
        <button type="button" class="btn-close" (click)="errorMessage = ''"></button>
      </div>

      <!-- Erfolgsmeldung -->
      <div *ngIf="successMessage" class="alert alert-success alert-dismissible fade show" role="alert">
        {{successMessage}}
        <button type="button" class="btn-close" (click)="successMessage = ''"></button>
      </div>
      
      <!-- Suchleiste und Neues Medikament Button -->
      <div class="row mb-4 align-items-center">
        <div class="col-md-6">
          <div class="search-container">
            <div class="input-group input-group-lg">
              <input 
                type="text" 
                class="form-control" 
                placeholder="Suche nach Medikamenten..." 
                [(ngModel)]="searchTerm"
                (input)="filterDrugs()"
              >
              <select class="form-select" style="max-width: 200px" [(ngModel)]="filterField" (change)="filterDrugs()">
                <option value="name">Name</option>
                <option value="description">Beschreibung</option>
                <option value="side_effects">Nebenwirkungen</option>
                <option value="all">Alle Felder</option>
              </select>
            </div>
          </div>
        </div>
        <div class="col-md-6 text-end">
          <button class="btn btn-primary btn-lg" (click)="openAddDialog()">
            <i class="bi bi-plus-circle me-2"></i>Neues Medikament
          </button>
        </div>
      </div>
      
      <!-- Medikamentenkarten -->
      <div class="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4">
        <div class="col" *ngFor="let drug of filteredDrugs">
          <div class="card h-100 drug-card">
            <div class="card-body">
              <h3 class="card-title drug-name">{{drug.name}}</h3>
              <div class="drug-info">
                <div class="info-section">
                  <h4 class="info-title">Beschreibung</h4>
                  <p class="info-text">{{drug.description}}</p>
                </div>
                <div class="info-section">
                  <h4 class="info-title">Nebenwirkungen</h4>
                  <p class="info-text">{{drug.side_effects}}</p>
                </div>
              </div>
            </div>
            <div class="card-footer">
              <div class="button-container">
                <button 
                  class="btn btn-action btn-edit" 
                  (click)="editDrug(drug)"
                  title="Bearbeiten"
                >
                  <i class="bi bi-pencil-square"></i>
                  <span class="action-text">Bearbeiten</span>
                </button>
                <button 
                  class="btn btn-action btn-delete" 
                  (click)="deleteDrug(drug.id)"
                  title="Löschen"
                >
                  <i class="bi bi-trash"></i>
                  <span class="action-text">Löschen</span>
                </button>
              </div>
            </div>
          </div>
        </div>
        <div class="col-12" *ngIf="filteredDrugs.length === 0">
          <div class="alert alert-info text-center">
            Keine Medikamente gefunden
          </div>
        </div>
      </div>
    </div>

    <!-- Dialog -->
    <app-drug-dialog
      *ngIf="showDialog"
      [drug]="selectedDrug"
      (save$)="saveDrug($event)"
      (cancel$)="closeDialog()"
    ></app-drug-dialog>
  `,
  styles: [`
    .container { 
      max-width: 1400px;
      min-height: 100vh;
      padding-bottom: 4rem;
    }
    .search-container {
      margin-bottom: 1rem;
    }
    .drug-card {
      transition: all 0.3s ease;
      border: 1px solid #e9ecef;
      box-shadow: 0 2px 4px rgba(0,0,0,0.05);
    }
    .drug-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0,0,0,0.1);
    }
    .drug-name {
      font-size: 1.5rem;
      color: #2c3e50;
      font-weight: bold;
      margin-bottom: 1.2rem;
      padding-bottom: 0.8rem;
      border-bottom: 2px solid #e9ecef;
    }
    .drug-info {
      display: flex;
      flex-direction: column;
      gap: 1.2rem;
    }
    .info-section {
      margin-bottom: 0.5rem;
    }
    .info-title {
      font-size: 1.1rem;
      color: #6c757d;
      margin-bottom: 0.5rem;
      font-weight: 500;
    }
    .info-text {
      font-size: 1.1rem;
      color: #2c3e50;
      line-height: 1.4;
      margin-bottom: 0;
    }
    .card-footer {
      background-color: #f8f9fa;
      border-top: 1px solid #e9ecef;
      padding: 1rem;
    }
    .button-container {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 0.5rem;
    }
    .btn-action {
      padding: 0.8rem;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      font-size: 1rem;
      font-weight: 500;
      transition: all 0.3s ease;
      width: 100%;
    }
    .btn-edit {
      background-color: #0d6efd;
      color: white;
      border: none;
      border-top-right-radius: 0;
      border-bottom-right-radius: 0;
    }
    .btn-delete {
      background-color: #dc3545;
      color: white;
      border: none;
      border-top-left-radius: 0;
      border-bottom-left-radius: 0;
    }
    .btn-edit:hover, .btn-delete:hover {
      transform: translateY(-2px);
      filter: brightness(110%);
    }
    .btn-edit:hover {
      box-shadow: 0 4px 8px rgba(13, 110, 253, 0.3);
    }
    .btn-delete:hover {
      box-shadow: 0 4px 8px rgba(220, 53, 69, 0.3);
    }
  `],
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule, DrugDialogComponent]
})
export class DrugListComponent implements OnInit {
  drugs: Drug[] = [];
  filteredDrugs: Drug[] = [];
  searchTerm: string = '';
  filterField: 'name' | 'description' | 'side_effects' | 'all' = 'name';
  errorMessage: string = '';
  successMessage: string = '';
  showDialog = false;
  selectedDrug?: Drug;

  constructor(private medicationService: MedicationService) {}

  ngOnInit() {
    this.loadDrugs();
  }

  loadDrugs() {
    this.medicationService.getDrugs().subscribe(
      (drugs) => {
        this.drugs = drugs;
        this.filterDrugs();
      },
      (error) => {
        console.error('Fehler beim Laden der Medikamente:', error);
        this.errorMessage = 'Fehler beim Laden der Medikamente: ' + (error.error?.error || error.message);
      }
    );
  }

  filterDrugs() {
    if (!this.searchTerm.trim()) {
      this.filteredDrugs = this.drugs;
      return;
    }

    const searchTermLower = this.searchTerm.toLowerCase().trim();
    
    this.filteredDrugs = this.drugs.filter(drug => {
      if (this.filterField === 'all') {
        return (
          (drug.name?.toLowerCase().includes(searchTermLower) || false) ||
          (drug.description?.toLowerCase().includes(searchTermLower) || false) ||
          (drug.side_effects?.toLowerCase().includes(searchTermLower) || false)
        );
      } else {
        const field = drug[this.filterField];
        return field?.toLowerCase().includes(searchTermLower) || false;
      }
    });
  }

  openAddDialog() {
    this.selectedDrug = undefined;
    this.showDialog = true;
  }

  editDrug(drug: Drug) {
    this.selectedDrug = drug;
    this.showDialog = true;
  }

  closeDialog() {
    this.showDialog = false;
    this.selectedDrug = undefined;
  }

  saveDrug(drug: Drug) {
    if (drug.id) {
      // Bearbeiten
      this.medicationService.updateDrug(drug.id, drug).subscribe(
        (updatedDrug) => {
          this.successMessage = 'Medikament erfolgreich aktualisiert';
          this.loadDrugs();
          this.closeDialog();
          setTimeout(() => this.successMessage = '', 3000);
        },
        (error) => {
          console.error('Fehler beim Aktualisieren des Medikaments:', error);
          this.errorMessage = error.error?.error || 'Fehler beim Aktualisieren des Medikaments';
        }
      );
    } else {
      // Neu hinzufügen
      this.medicationService.createDrug(drug).subscribe(
        (newDrug) => {
          this.successMessage = 'Medikament erfolgreich hinzugefügt';
          this.loadDrugs();
          this.closeDialog();
          setTimeout(() => this.successMessage = '', 3000);
        },
        (error) => {
          console.error('Fehler beim Hinzufügen des Medikaments:', error);
          this.errorMessage = error.error?.error || 'Fehler beim Hinzufügen des Medikaments';
        }
      );
    }
  }

  deleteDrug(id: number | undefined) {
    if (!id) return;
    
    if (confirm('Möchten Sie dieses Medikament wirklich löschen?')) {
      this.medicationService.deleteDrug(id).subscribe(
        (response) => {
          this.successMessage = 'Medikament erfolgreich gelöscht';
          this.loadDrugs();
          setTimeout(() => this.successMessage = '', 3000);
        },
        (error) => {
          console.error('Fehler beim Löschen des Medikaments:', error);
          this.errorMessage = error.error?.error || 'Fehler beim Löschen des Medikaments';
        }
      );
    }
  }
} 