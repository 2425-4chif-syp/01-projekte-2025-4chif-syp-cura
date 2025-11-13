import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { CalendarDay } from '../models/calendar-day.model';
import { DailyStatus } from '../models/daily-status.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDailyStatus(patientId: number, year: number, month: number): Observable<DailyStatus[]> {
    return this.http.get<DailyStatus[]>(
      `${this.API_URL}/MedicationStatus/patient/${patientId}/month/${year}/${month}`
    );
  }

  generateCalendarFromStatus(statusData: DailyStatus[]): CalendarDay[] {
    const calendarDays: CalendarDay[] = [];
    
    if (statusData.length === 0) {
      return this.generateEmptyCalendar();
    }

    const firstDate = new Date(statusData[0].date);
    const year = firstDate.getFullYear();
    const month = firstDate.getMonth();
    const firstDay = new Date(year, month, 1);
    const startDayOfWeek = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;

    for (let i = 0; i < startDayOfWeek; i++) {
      calendarDays.push({ day: 0, date: '' });
    }

    for (const status of statusData) {
      const date = new Date(status.date);
      const dayData: CalendarDay = {
        day: date.getDate(),
        date: status.date
      };

      if (status.status === 'checked') dayData.checked = true;
      else if (status.status === 'partial') dayData.missed = true;
      else if (status.status === 'missed') dayData.missed = true;

      calendarDays.push(dayData);
    }

    return calendarDays;
  }

  generateEmptyCalendar(): CalendarDay[] {
    const calendarDays: CalendarDay[] = [];
    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const startDayOfWeek = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;

    for (let i = 0; i < startDayOfWeek; i++) {
      calendarDays.push({ day: 0, date: '' });
    }

    for (let day = 1; day <= lastDay.getDate(); day++) {
      const dateStr = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
      calendarDays.push({ day, date: dateStr });
    }

    return calendarDays;
  }

  calculateIntakeQuote(statusData: DailyStatus[]): number {
    const daysWithScheduled = statusData.filter(d => d.scheduled > 0);
    if (daysWithScheduled.length === 0) return 0;

    const totalScheduled = daysWithScheduled.reduce((sum, d) => sum + d.scheduled, 0);
    const totalTaken = daysWithScheduled.reduce((sum, d) => sum + d.taken, 0);

    return Math.round((totalTaken / totalScheduled) * 100);
  }

  getCurrentMonth(): string {
    return new Date().toLocaleDateString('de-DE', { month: 'long', year: 'numeric' });
  }
}
