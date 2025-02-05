export type WeekDay = 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday' | 'Sunday';

export interface Drug {
    id?: number;
    name: string;
    side_effects?: string;
    description?: string;
}

export interface MedicationPlan {
    id?: number;
    drug_id: number;
    day: WeekDay;
    time: string;
    drug?: Drug;  // Für die Anzeige des zugehörigen Medikaments
} 