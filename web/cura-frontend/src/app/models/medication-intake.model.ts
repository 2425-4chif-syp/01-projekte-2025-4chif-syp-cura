/**
 * Represents a medication intake event
 * Tracks individual medication intake with timestamp, quantity, and optional medication plan reference
 */
export interface MedicationIntake {
  /** Unique identifier */
  id?: number;
  
  /** Patient ID */
  patientId: number;
  
  /** Reference to medication plan (nullable) */
  medicationPlanId?: number;
  
  /** Exact timestamp of intake (e.g., "2026-01-15T09:31:00") */
  intakeTime: string;
  
  /** Number of pills/doses taken */
  quantity: number;
  
  /** RFID tag used to record the intake (optional) */
  rfidTag?: string;
  
  notes?: string;
  
  // ===== Calculated/Backward-Compatible Properties =====
  // These are calculated on the backend from intakeTime
  
  /** Date portion of intakeTime (e.g., "2026-01-15") - for backward compatibility */
  intakeDate?: string;
  
  /** Time of day flag: 1=Morning, 2=Noon, 8=Evening, 16=Night - calculated from hour */
  dayTimeFlag?: number;
  
  /** Human-readable time of day (Morning/Noon/Evening/Night) */
  timeLabel?: string;
}
