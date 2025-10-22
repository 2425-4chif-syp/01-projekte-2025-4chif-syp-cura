export interface MedicationPlan {
  id: number;
  patientId: number;
  medicationId: number;
  caregiverId: number;
  weekdayFlags: number;
  dayTimeFlags: number;
  quantity: number;
  validFrom: string;
  validTo: string;
  notes: string;
  isActive: boolean;
}
