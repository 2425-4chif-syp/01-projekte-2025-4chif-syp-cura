import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';  // Importiere FormsModule
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-medication-plan-form',
  standalone: true,
  imports: [CommonModule, FormsModule],  // Füge FormsModule hier hinzu
  templateUrl: './medication-plan-form.component.html',
  styleUrls: ['./medication-plan-form.component.css']
})
export class MedicationPlanFormComponent {
  medicationPlan = {
    medication: '',
    dosage: '',
    frequency: ''
  };

  onSubmit() {
    console.log('Medikamentenplan hinzugefügt:', this.medicationPlan);
  }
}