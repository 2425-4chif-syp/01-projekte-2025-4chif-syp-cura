import { Injectable } from '@angular/core';
import { CalendarDay } from '../models/calendar-day.model';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  generateCalendar(): CalendarDay[] {
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
      const dayData: CalendarDay = { day, date: dateStr };
      
      if (day % 7 === 1) dayData.checked = true;
      else if (day % 7 === 2) dayData.partial = true;
      else if (day % 7 === 5) dayData.missed = true;
      
      calendarDays.push(dayData);
    }

    return calendarDays;
  }

  getCurrentMonth(): string {
    return new Date().toLocaleDateString('de-DE', { month: 'long', year: 'numeric' });
  }
}
