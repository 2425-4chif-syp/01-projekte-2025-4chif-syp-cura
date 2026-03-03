import { Routes } from '@angular/router';
import { MedicationPlanEditorComponent } from './components/medication-plan-editor/medication-plan-editor.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';

export const routes: Routes = [
  {
    path: '',
    component: DashboardComponent
  },
  {
    path: 'medication-plan-editor',
    component: MedicationPlanEditorComponent
  }
];
