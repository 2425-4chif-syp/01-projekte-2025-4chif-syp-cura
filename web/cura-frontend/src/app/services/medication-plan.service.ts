import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, map, of, switchMap } from 'rxjs';
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

  // Get all available medications
  getAllMedications(): Observable<Medication[]> {
    return this.http.get<Medication[]>(`${this.API_URL}/Medications`);
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

  /**
   * Erstellt mehrere Medikamentenpläne aus dem Wizard-Schedule
   * @param weekSchedule Map von Tag-Index -> Map von Tageszeit -> Medikamente
   * @param patientId Patient ID
   * @returns Observable der erstellten Pläne
   */
  createWeeklyMedicationPlans(
    weekSchedule: Map<number, Map<string, Array<{ medicationId: number | null; name: string; dosage: number; dosageUnit: string }>>>,
    patientId: number
  ): Observable<any[]> {
    const timeOfDayMap: { [key: string]: number } = {
      'MORNING': 1,
      'NOON': 2,
      'AFTERNOON': 4,
      'EVENING': 8
    };

    const weekdayFlagsArray = [2, 4, 8, 16, 32, 64, 1]; // Mo=2, Di=4, Mi=8, Do=16, Fr=32, Sa=64, So=1

    // Sammle alle neuen Medikamente die erstellt werden müssen
    const newMedicationsToCreate = new Set<string>();
    const medicationNameToIdMap = new Map<string, number>();

    // Gruppiere Medikamente nach medicationId + dosage + timeOfDay Kombination
    const medicationGroups = new Map<string, { 
      medicationId: number | null;
      medicationName: string;
      dosage: number; 
      dosageUnit: string;
      weekdayFlags: number; 
      dayTimeFlags: number 
    }>();

    weekSchedule.forEach((dayMap, dayIndex) => {
      const weekdayFlag = weekdayFlagsArray[dayIndex];
      
      dayMap.forEach((medications, timeOfDay) => {
        const dayTimeFlag = timeOfDayMap[timeOfDay];
        
        medications.forEach(med => {
          if (!med.medicationId && !med.name.trim()) return;
          
          // Wenn kein medicationId, merke Name für spätere Erstellung
          if (!med.medicationId && med.name.trim()) {
            newMedicationsToCreate.add(med.name.trim());
          }
          
          // Erstelle eindeutigen Key
          const medKey = med.medicationId ? `id_${med.medicationId}` : `name_${med.name.trim()}`;
          const key = `${medKey}_${med.dosage}_${med.dosageUnit}_${timeOfDay}`;
          
          if (medicationGroups.has(key)) {
            // Füge Wochentag hinzu
            const existing = medicationGroups.get(key)!;
            existing.weekdayFlags |= weekdayFlag;
          } else {
            // Neuer Eintrag
            medicationGroups.set(key, {
              medicationId: med.medicationId,
              medicationName: med.name.trim(),
              dosage: med.dosage,
              dosageUnit: med.dosageUnit,
              weekdayFlags: weekdayFlag,
              dayTimeFlags: dayTimeFlag
            });
          }
        });
      });
    });

    // Erstelle zuerst neue Medikamente
    const createMedicationRequests = Array.from(newMedicationsToCreate).map(name =>
      this.http.post<Medication>(`${this.API_URL}/Medications`, { name, activeIngredient: null }).pipe(
        map(createdMed => {
          medicationNameToIdMap.set(name, createdMed.id);
          return createdMed;
        })
      )
    );

    // Wenn es neue Medikamente gibt, erstelle sie zuerst
    const medicationCreation$ = createMedicationRequests.length > 0 
      ? forkJoin(createMedicationRequests) 
      : of([]);

    return medicationCreation$.pipe(
      map(() => {
        // Jetzt haben wir IDs für alle neuen Medikamente
        const planRequests: Observable<any>[] = [];
        const validFrom = new Date();
        validFrom.setHours(0, 0, 0, 0);

        medicationGroups.forEach((group) => {
          // Hole die richtige medicationId
          const medId = group.medicationId || medicationNameToIdMap.get(group.medicationName) || 0;
          
          if (medId === 0) {
            console.error('Keine gültige Medication ID für', group.medicationName);
            return;
          }

          const plan = {
            PatientId: patientId,
            MedicationId: medId,
            CaregiverId: null,
            WeekdayFlags: group.weekdayFlags,
            DayTimeFlags: group.dayTimeFlags,
            Quantity: group.dosage,
            ValidFrom: validFrom.toISOString(),
            ValidTo: null,
            Notes: `${group.dosageUnit}`,
            IsActive: true,
            Patient: {} as any,  // Leeres Objekt für Navigation Property
            Medication: {} as any  // Leeres Objekt für Navigation Property
          };
          
          console.log('Erstelle Plan:', plan);
          planRequests.push(this.http.post(`${this.API_URL}/MedicationPlans`, plan));
        });

        return planRequests;
      }),
      switchMap(planRequests => planRequests.length > 0 ? forkJoin(planRequests) : of([]))
    );
  }
}

