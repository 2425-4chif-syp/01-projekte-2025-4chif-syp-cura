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

  /**
   * Get plan groups for a patient (grouped by ValidFrom + ValidTo)
   * @param patientId Patient ID
   * @returns Array of plan groups with metadata
   */
  getPatientPlanGroups(patientId: number): Observable<{id: string; name: string; patientName: string; validFrom: string; validTo: string | null; medicationCount: number; isCurrentlyActive: boolean}[]> {
    return this.getMedicationPlans(patientId).pipe(
      map(plans => {
        // Gruppiere nach ValidFrom + ValidTo Kombination
        const groups = new Map<string, MedicationPlan[]>();
        
        plans.forEach(plan => {
          const validFromDate = plan.validFrom.split('T')[0]; // YYYY-MM-DD
          const validToDate = plan.validTo ? plan.validTo.split('T')[0] : 'unbegrenzt';
          const groupKey = `${validFromDate}_${validToDate}`;
          
          if (!groups.has(groupKey)) {
            groups.set(groupKey, []);
          }
          groups.get(groupKey)!.push(plan);
        });
        
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        // Konvertiere zu Array und sortiere (neueste zuerst)
        return Array.from(groups.entries())
          .map(([groupKey, planList]) => {
            const validFrom = new Date(planList[0].validFrom);
            const validTo = planList[0].validTo ? new Date(planList[0].validTo) : null;
            
            // Prüfe ob aktuell gültig
            const isCurrentlyActive = planList.some(p => {
              const vf = new Date(p.validFrom);
              vf.setHours(0, 0, 0, 0);
              const vt = p.validTo ? new Date(p.validTo) : new Date(2099, 11, 31);
              vt.setHours(23, 59, 59, 999);
              return p.isActive && vf <= today && today <= vt;
            });
            
            // Gruppennamen erstellen
            let name = '';
            if (validTo) {
              name = `Medikamentenplan ${this.formatDateRange(planList[0].validFrom, planList[0].validTo!)}`;
            } else {
              name = `Medikamentenplan ab ${this.formatDate(planList[0].validFrom)}`;
            }
            
            return {
              id: groupKey,
              name: name,
              patientName: 'Patient',
              validFrom: planList[0].validFrom.split('T')[0],
              validTo: planList[0].validTo ? planList[0].validTo.split('T')[0] : null,
              medicationCount: planList.length,
              isCurrentlyActive: isCurrentlyActive
            };
          })
          .sort((a, b) => {
            // Aktive zuerst, dann nach Datum absteigend
            if (a.isCurrentlyActive && !b.isCurrentlyActive) return -1;
            if (!a.isCurrentlyActive && b.isCurrentlyActive) return 1;
            return b.validFrom.localeCompare(a.validFrom);
          });
      })
    );
  }

  /**
   * Get medication plans by ValidFrom date (for a specific plan group)
   * @param patientId Patient ID
   * @param validFromDate ValidFrom date in YYYY-MM-DD format
   * @returns Medication plans starting on that date
   */
  getMedicationPlansByDate(patientId: number, validFromDate: string, validToDate: string | null = null): Observable<MedicationPlan[]> {
    return this.getMedicationPlans(patientId).pipe(
      map(plans => plans.filter(p => {
        const planValidFrom = p.validFrom.split('T')[0];
        const planValidTo = p.validTo ? p.validTo.split('T')[0] : null;
        
        // Filter nach ValidFrom und ValidTo
        if (validToDate) {
          return planValidFrom === validFromDate && planValidTo === validToDate;
        } else {
          return planValidFrom === validFromDate && !planValidTo;
        }
      }))
    );
  }

  /**
   * Get all medication plans that are valid for a specific date
   * @param patientId Patient ID
   * @param date Date to check (default: today)
   * @returns All plans valid on that date
   */
  getActivePlansForDate(patientId: number, date: Date = new Date()): Observable<MedicationPlan[]> {
    const checkDate = new Date(date);
    checkDate.setHours(12, 0, 0, 0); // Mittag zur sicheren Prüfung
    
    return this.getMedicationPlans(patientId).pipe(
      map(plans => plans.filter(plan => {
        const validFrom = new Date(plan.validFrom);
        validFrom.setHours(0, 0, 0, 0);
        
        const validTo = plan.validTo ? new Date(plan.validTo) : new Date(2099, 11, 31);
        validTo.setHours(23, 59, 59, 999);
        
        return plan.isActive && validFrom <= checkDate && checkDate <= validTo;
      }))
    );
  }

  private formatDate(isoDate: string): string {
    const date = new Date(isoDate);
    return date.toLocaleDateString('de-DE', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }

  private formatDateRange(isoFrom: string, isoTo: string): string {
    const from = new Date(isoFrom);
    const to = new Date(isoTo);
    return `${from.toLocaleDateString('de-DE', { day: '2-digit', month: '2-digit' })} - ${to.toLocaleDateString('de-DE', { day: '2-digit', month: '2-digit', year: 'numeric' })}`;
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
      { flag: 8, label: 'Abend' },
      { flag: 16, label: 'Nachts' }
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
   * Erstellt eine transponierte Medikamententabelle (Tage als Zeilen, Zeiten als Spalten)
   */
  buildMedicationTableTransposed(plans: MedicationPlan[], medicationNames: Map<number, string>): { dayLabel: string; times: string[] }[] {
    const timeLabels = [
      { flag: 1, label: 'Morgen' },
      { flag: 2, label: 'Mittag' },
      { flag: 8, label: 'Abend' },
      { flag: 16, label: 'Nachts' }
    ];

    // WICHTIG: Reihenfolge muss Mo, Di, Mi, Do, Fr, Sa, So sein!
    // Mo=2, Di=4, Mi=8, Do=16, Fr=32, Sa=64, So=1
    const weekdays = [
      { flag: 2, label: 'Mo' },
      { flag: 4, label: 'Di' },
      { flag: 8, label: 'Mi' },
      { flag: 16, label: 'Do' },
      { flag: 32, label: 'Fr' },
      { flag: 64, label: 'Sa' },
      { flag: 1, label: 'So' }
    ];

    const rows: { dayLabel: string; times: string[] }[] = [];

    // Für jeden Wochentag
    for (const dayConfig of weekdays) {
      const times: string[] = [];

      // Für jede Tageszeit (Morgen, Mittag, Nachmittag, Abend)
      for (const timeConfig of timeLabels) {
        const medicationSet = new Set<string>();

        // Sammle alle Medikamente für diese Tag + Zeit Kombination
        for (const plan of plans) {
          const hasTime = (plan.dayTimeFlags & timeConfig.flag) !== 0;
          const hasDay = (plan.weekdayFlags & dayConfig.flag) !== 0;
          
          if (hasTime && hasDay) {
            const medName = medicationNames.get(plan.medicationId) || `Med${plan.medicationId}`;
            medicationSet.add(medName);
          }
        }

        // Konvertiere Set zu Array und verbinde mit Komma, oder "-" wenn leer
        times.push(medicationSet.size > 0 ? Array.from(medicationSet).join(', ') : '-');
      }

      rows.push({ dayLabel: dayConfig.label, times });
    }

    return rows;
  }

  /**
   * Erstellt mehrere Medikamentenpläne aus dem Wizard-Schedule
   * @param weekSchedule Map von Tag-Index -> Map von Tageszeit -> Medikamente
   * @param patientId Patient ID
   * @param validFromDate Start date for the plan (default: today)
   * @param validToDate End date for the plan (null = unlimited)
   * @returns Observable der erstellten Pläne
   */
  createWeeklyMedicationPlans(
    weekSchedule: Map<number, Map<string, Array<{ medicationId: number | null; name: string; dosage: number; dosageUnit: string }>>>,
    patientId: number,
    validFromDate: Date = new Date(),
    validToDate: Date | null = null
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
        const validFrom = new Date(validFromDate);
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
            ValidTo: validToDate ? validToDate.toISOString() : null,
            Notes: `${group.dosageUnit}`,
            IsActive: true
          };
          
          console.log('Erstelle Plan:', plan);
          planRequests.push(this.http.post(`${this.API_URL}/MedicationPlans`, plan));
        });

        return planRequests;
      }),
      switchMap(planRequests => planRequests.length > 0 ? forkJoin(planRequests) : of([]))
    );
  }

  // Deactivate all currently active plans for a patient by setting ValidTo date
  deactivateActivePlans(patientId: number, validToDate: string): Observable<any> {
    return this.getMedicationPlans(patientId).pipe(
      switchMap(plans => {
        // Filter nur aktive Pläne
        const activePlans = plans.filter(p => p.isActive);
        
        if (activePlans.length === 0) {
          console.log('ℹ️ Keine aktiven Pläne zum Deaktivieren');
          return of(null);
        }
        
        console.log(`🔄 Deaktiviere ${activePlans.length} Pläne...`);
        
        // Setze ValidTo und IsActive
        const updateRequests = activePlans.map(plan => {
          const payload = {
            Id: plan.id,
            PatientId: plan.patientId,
            MedicationId: plan.medicationId,
            CaregiverId: plan.caregiverId,
            WeekdayFlags: plan.weekdayFlags,
            DayTimeFlags: plan.dayTimeFlags,
            Quantity: plan.quantity,
            ValidFrom: plan.validFrom,
            ValidTo: validToDate,
            Notes: plan.notes,
            IsActive: false
            // Navigation Properties KOMPLETT weglassen (nicht null, einfach nicht vorhanden)
          };
          console.log('📝 Update Plan:', plan.id, 'auf inactive');
          return this.http.put(`${this.API_URL}/MedicationPlans/${plan.id}`, payload);
        });
        
        return forkJoin(updateRequests);
      })
    );
  }

  // Delete all medication plans for a patient
  deleteAllPlansForPatient(patientId: number): Observable<any> {
    // Zuerst alle Pläne des Patienten laden
    return this.getMedicationPlans(patientId).pipe(
      switchMap(plans => {
        if (plans.length === 0) {
          return of(null);
        }
        // Jeden Plan einzeln löschen
        const deleteRequests = plans.map(plan => 
          this.http.delete(`${this.API_URL}/MedicationPlans/${plan.id}`)
        );
        return forkJoin(deleteRequests);
      })
    );
  }

  // Create multiple medication plans at once
  createMedicationPlans(plans: Partial<MedicationPlan>[]): Observable<MedicationPlan[]> {
    const requests = plans.map(plan => {
      const validFromDate = new Date(plan.validFrom!);
      const validToDate = plan.validTo ? new Date(plan.validTo) : new Date(2099, 11, 31);
      
      const payload = {
        PatientId: plan.patientId,
        MedicationId: plan.medicationId,
        CaregiverId: plan.caregiverId || 1,
        WeekdayFlags: plan.weekdayFlags,
        DayTimeFlags: plan.dayTimeFlags,
        Quantity: plan.quantity,
        ValidFrom: validFromDate.toISOString(),
        ValidTo: validToDate.toISOString(),
        Notes: plan.notes || '',
        IsActive: true
      };
      
      console.log('📤 POST Payload:', payload);
      return this.http.post<MedicationPlan>(`${this.API_URL}/MedicationPlans`, payload);
    });

    return requests.length > 0 ? forkJoin(requests) : of([]);
  }
}

