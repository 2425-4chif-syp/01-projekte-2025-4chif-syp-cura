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

    const rows: { timeLabel: string; days: string[] }[] = [];
    const medicationsByTime = new Map<number, Map<number, string>>();

    for (const plan of plans) {
      for (let timeFlag = 1; timeFlag <= 8; timeFlag *= 2) {
        if (plan.dayTimeFlags & timeFlag) {
          if (!medicationsByTime.has(timeFlag)) {
            medicationsByTime.set(timeFlag, new Map());
          }
          const timeMap = medicationsByTime.get(timeFlag)!;
          
          for (let dayFlag = 1; dayFlag <= 64; dayFlag *= 2) {
            if (plan.weekdayFlags & dayFlag) {
              const dayIndex = Math.log2(dayFlag);
              const medName = medicationNames.get(plan.medicationId) || `Med${plan.medicationId}`;
              timeMap.set(dayIndex, medName);
            }
          }
        }
      }
    }

    for (const timeConfig of timeLabels) {
      const days: string[] = [];
      const timeMap = medicationsByTime.get(timeConfig.flag);
      
      for (let dayIndex = 0; dayIndex < 7; dayIndex++) {
        days.push(timeMap?.get(dayIndex) || '');
      }
      
      rows.push({ timeLabel: timeConfig.label, days });
    }

    return rows;
  }
}
