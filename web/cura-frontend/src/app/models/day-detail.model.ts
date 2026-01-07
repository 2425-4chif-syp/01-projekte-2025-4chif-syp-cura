export interface DayDetail {
  medicationPlanId: number;
  medicationName: string;
  timeLabel: string;
  timeSlotFlag: number;
  quantity: number;
  wasTaken: boolean;
  intakeTime?: string;
  actualQuantity?: number;
  notes?: string;
}
