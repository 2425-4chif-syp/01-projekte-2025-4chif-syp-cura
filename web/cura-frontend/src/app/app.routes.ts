import { Routes } from '@angular/router';
import { MedicationPlanEditorComponent } from './components/medication-plan-editor/medication-plan-editor.component';

export const routes: Routes = [
  {
    path: 'medication-plan-editor',
    component: MedicationPlanEditorComponent
  },
  {
    path: '',
    redirectTo: '/medication-plan-editor',
    pathMatch: 'full'
  }
];
