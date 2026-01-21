import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, map, of } from 'rxjs';
import { MedicationPlan } from '../models/medication-plan.model';
import { Medication } from '../models/medication.model';
import { MedicationIntake } from '../models/medication-intake.model';
import { DayDetail } from '../models/day-detail.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MedicationPlanService {
  private readonly API_URL = environment.apiUrl;
  private medicationCache = new Map<number, string>();

  constructor(private http: HttpClient) {}

  // Get all patients with medication plans for selection
  getAllAvailablePlans(): Observable<{ id: number; name: string; patientName: string }[]> {
    return this.http.get<{ id: number; name: string; planCount: number; hasActivePlans: boolean }[]>(
      `${this.API_URL}/MedicationPlans/patients-with-plans`
    ).pipe(
      map(patients => patients.map(p => ({
        id: p.id,
        name: `Medikamentenplan ${p.id}`,
        patientName: p.name
      })))
    );
  }

  getMedicationPlans(patientId: number): Observable<MedicationPlan[]> {
    return this.http.get<MedicationPlan[]>(`${this.API_URL}/MedicationPlans/patient/${patientId}`);
  }

  getMedicationName(medicationId: number): Observable<string> {
    if (this.medicationCache.has(medicationId)) {
      return of(this.medicationCache.get(medicationId)!);
    }

    return this.http.get<Medication>(`${this.API_URL}/Medications/${medicationId}`).pipe(
      map(med => {
        this.medicationCache.set(medicationId, med.name);
        return med.name;
      })
    );
  }

  /**
   * Get all drawer opening logs for a specific date
   * @param patientId Patient ID
   * @param date Date in YYYY-MM-DD format
   * @returns Array of drawer opening events (which times of day were opened)
   */
  getMedicationIntakesForDate(patientId: number, date: string): Observable<MedicationIntake[]> {
    return this.http.get<MedicationIntake[]>(
      `${this.API_URL}/MedicationIntakes/patient/${patientId}/date/${date}`
    );
  }

  /**
   * Get all drawer opening logs for a date range
   * @param patientId Patient ID
   * @param startDate Start date in YYYY-MM-DD format
   * @param endDate End date in YYYY-MM-DD format
   */
  getMedicationIntakesForRange(patientId: number, startDate: string, endDate: string): Observable<MedicationIntake[]> {
    return this.http.get<MedicationIntake[]>(
      `${this.API_URL}/MedicationIntakes/patient/${patientId}/range?startDate=${startDate}&endDate=${endDate}`
    );
  }

  /**
   * Get day details showing all scheduled medications and their status
   * @param patientId Patient ID
   * @param year Year
   * @param month Month (1-12)
   * @param day Day of month
   * @returns Array of medications with status (taken/missed)
   */
  getDayDetails(patientId: number, year: number, month: number, day: number): Observable<DayDetail[]> {
    return this.http.get<DayDetail[]>(
      `${this.API_URL}/MedicationStatus/patient/${patientId}/day/${year}/${month}/${day}`
    );
  }

  buildMedicationTable(plans: MedicationPlan[], medicationNames: Map<number, string>): { timeLabel: string; days: string[] }[] {
    const timeLabels = [
      { flag: 1, label: 'Morgen' },
      { flag: 2, label: 'Mittag' },
      { flag: 4, label: 'Nachmittag' },
      { flag: 8, label: 'Abend' }
    ];

    // WICHTIG: Reihenfolge muss Mo, Di, Mi, Do, Fr, Sa, So sein!
    // Mo=2, Di=4, Mi=8, Do=16, Fr=32, Sa=64, So=1
    const weekdayFlags = [2, 4, 8, 16, 32, 64, 1]; // Mo, Di, Mi, Do, Fr, Sa, So

    const rows: { timeLabel: string; days: string[] }[] = [];

    // Für jede Tageszeit
    for (const timeConfig of timeLabels) {
      const days: string[] = [];

      // Für jeden Wochentag (Mo-So)
      for (let dayIndex = 0; dayIndex < 7; dayIndex++) {
        const dayFlag = weekdayFlags[dayIndex];
        const medicationSet = new Set<string>();

        // Sammle alle Medikamente für diese Zeit + Tag Kombination
        for (const plan of plans) {
          const hasTime = (plan.dayTimeFlags & timeConfig.flag) !== 0;
          const hasDay = (plan.weekdayFlags & dayFlag) !== 0;
          
          if (hasTime && hasDay) {
            const medName = medicationNames.get(plan.medicationId) || `Med${plan.medicationId}`;
            medicationSet.add(medName);
          }
        }

        // Konvertiere Set zu Array und verbinde mit Komma, oder "-" wenn leer
        days.push(medicationSet.size > 0 ? Array.from(medicationSet).join(', ') : '-');
      }

      rows.push({ timeLabel: timeConfig.label, days });
    }

    return rows;
  }
}
