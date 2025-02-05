import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MedicationLog {
  id: number;
  timestamp: string;
  rfid: string;
  medication_name: string;
  patient_name: string;
  taken_time?: string;
}

export interface WeeklyStatistic {
  date: string;
  medication_name: string;
  taken_count: number;
}

@Injectable({
  providedIn: 'root'
})
export class MedicationLogsService {
  private apiUrl = 'http://localhost:3000/api';

  constructor(private http: HttpClient) {}

  getAllLogs(): Observable<MedicationLog[]> {
    return this.http.get<MedicationLog[]>(`${this.apiUrl}/medication-logs`);
  }

  getTodayLogs(): Observable<MedicationLog[]> {
    return this.http.get<MedicationLog[]>(`${this.apiUrl}/medication-logs/today`);
  }

  getWeeklyStatistics(): Observable<WeeklyStatistic[]> {
    return this.http.get<WeeklyStatistic[]>(`${this.apiUrl}/medication-logs/weekly`);
  }
} 