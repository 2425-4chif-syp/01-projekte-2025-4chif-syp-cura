import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, map, of } from 'rxjs';
import { MedicationPlan } from '../models/medication-plan.model';
import { Medication } from '../models/medication.model';
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
