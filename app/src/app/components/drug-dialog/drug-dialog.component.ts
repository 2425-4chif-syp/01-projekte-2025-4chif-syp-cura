import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Drug } from '../../models/drug.interface';

@Component({
  selector: 'app-drug-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal fade show" style="display: block; background-color: rgba(0,0,0,0.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ drug?.id ? 'Medikament bearbeiten' : 'Neues Medikament' }}</h5>
            <button type="button" class="btn-close" (click)="cancel()"></button>
          </div>
          <div class="modal-body">
            <form #drugForm="ngForm">
              <div class="mb-3">
                <label for="name" class="form-label">Name*</label>
                <input 
                  type="text" 
                  class="form-control" 
                  id="name" 
                  name="name"
                  [(ngModel)]="editedDrug.name"
                  required
                >
              </div>
              <div class="mb-3">
                <label for="description" class="form-label">Beschreibung</label>
                <textarea 
                  class="form-control" 
                  id="description" 
                  name="description"
                  [(ngModel)]="editedDrug.description"
                  rows="3"
                ></textarea>
              </div>
              <div class="mb-3">
                <label for="side_effects" class="form-label">Nebenwirkungen</label>
                <textarea 
                  class="form-control" 
                  id="side_effects" 
                  name="side_effects"
                  [(ngModel)]="editedDrug.side_effects"
                  rows="3"
                ></textarea>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" (click)="cancel()">Abbrechen</button>
            <button 
              type="button" 
              class="btn btn-primary" 
              (click)="save()"
              [disabled]="!editedDrug.name"
            >
              Speichern
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class DrugDialogComponent {
  @Input() drug?: Drug;
  @Output() save$ = new EventEmitter<Drug>();
  @Output() cancel$ = new EventEmitter<void>();

  editedDrug: Drug = {
    name: '',
    description: '',
    side_effects: ''
  };

  ngOnInit() {
    if (this.drug) {
      this.editedDrug = { ...this.drug };
    }
  }

  save() {
    if (this.editedDrug.name) {
      this.save$.emit(this.editedDrug);
    }
  }

  cancel() {
    this.cancel$.emit();
  }
} 