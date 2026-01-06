/**
 * Represents a drawer opening event
 * System can only track: Date + Time of Day (Morning/Noon/Afternoon/Evening)
 */
export interface MedicationIntake {
  /** Date when drawer was opened (e.g., "2026-01-15") */
  intakeDate: string;
  
  /** Time of day when drawer was opened: 1=Morning, 2=Noon, 4=Afternoon, 8=Evening */
  dayTimeFlag: number;
  
  /** Human-readable time of day */
  timeLabel: string;
  
  /** Exact timestamp when drawer was opened */
  openedAt: string;
  
  /** RFID tag used to open the drawer */
  rfidTag?: string;
  
  notes?: string;
}
