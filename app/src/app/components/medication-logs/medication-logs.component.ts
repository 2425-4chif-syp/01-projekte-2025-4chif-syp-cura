import { Component, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { MedicationLogsService, MedicationLog } from '../../services/medication-logs.service';

interface TimeSlot {
  name: string;
  startTime: string;
  endTime: string;
  medications: MedicationLog[];
}

@Component({
  selector: 'app-medication-logs',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  providers: [MedicationLogsService],
  template: `
    <div class="container mt-4">
      <h2 class="mb-4">Medikamenteneinnahme für {{getCurrentDayName()}}</h2>
      <div class="row">
        <div class="col-md-6 col-lg-3 mb-4" *ngFor="let slot of timeSlots">
          <div class="card h-100">
            <div class="card-header bg-primary text-white">
              <h3 class="mb-0">{{slot.name}}</h3>
              <small>{{slot.startTime}} - {{slot.endTime}} Uhr</small>
            </div>
            <div class="card-body">
              <div class="medication-list">
                <div *ngFor="let med of getMedicationsForSlot(slot)" 
                     class="medication-item"
                     [class.taken]="isMedicationTaken(med, slot)">
                  <i class="bi" [class.bi-check-circle-fill]="isMedicationTaken(med, slot)"
                     [class.bi-circle]="!isMedicationTaken(med, slot)"></i>
                  <span class="medication-name">{{med.medication_name}}</span>
                  <span *ngIf="isMedicationTaken(med, slot)" class="taken-time">
                    {{formatTime(med.taken_time)}}
                  </span>
                </div>
                <div *ngIf="getMedicationsForSlot(slot).length === 0" class="no-medications">
                  Keine Medikamente in diesem Zeitfenster
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .card {
      box-shadow: 0 4px 8px rgba(0,0,0,0.1);
      border: none;
      border-radius: 12px;
      transition: transform 0.2s;
    }
    .card:hover {
      transform: translateY(-5px);
    }
    .card-header {
      border-radius: 12px 12px 0 0;
      padding: 1.5rem;
    }
    .card-header h3 {
      font-size: 1.5rem;
      font-weight: 600;
    }
    .card-body {
      padding: 1.5rem;
    }
    .medication-list {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
    .medication-item {
      display: flex;
      align-items: center;
      padding: 1rem;
      background-color: #f8f9fa;
      border-radius: 8px;
      transition: all 0.3s ease;
    }
    .medication-item.taken {
      background-color: #d4edda;
      color: #155724;
    }
    .medication-item i {
      font-size: 1.5rem;
      margin-right: 1rem;
    }
    .bi-check-circle-fill {
      color: #28a745;
    }
    .bi-circle {
      color: #6c757d;
    }
    .medication-name {
      font-size: 1.1rem;
      font-weight: 500;
      flex-grow: 1;
    }
    .taken-time {
      font-size: 0.9rem;
      color: #666;
      margin-left: 1rem;
    }
    .no-medications {
      text-align: center;
      color: #6c757d;
      padding: 1rem;
      font-style: italic;
    }
  `]
})
export class MedicationLogsComponent implements OnInit {
  todayLogs: MedicationLog[] = [];
  private updateInterval: any;

  timeSlots: TimeSlot[] = [
    { name: 'Morgens', startTime: '06:00', endTime: '10:00', medications: [] },
    { name: 'Mittags', startTime: '11:00', endTime: '14:00', medications: [] },
    { name: 'Abends', startTime: '17:00', endTime: '20:00', medications: [] },
    { name: 'Nachts', startTime: '20:00', endTime: '23:00', medications: [] }
  ];

  constructor(
    private logsService: MedicationLogsService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit() {
    this.loadTodayLogs();
    
    if (isPlatformBrowser(this.platformId)) {
      this.updateInterval = setInterval(() => {
        this.loadTodayLogs();
      }, 30000);
    }
  }

  ngOnDestroy() {
    if (isPlatformBrowser(this.platformId) && this.updateInterval) {
      clearInterval(this.updateInterval);
    }
  }

  loadTodayLogs() {
    this.logsService.getTodayLogs().subscribe({
      next: (logs) => {
        console.log('Empfangene Logs:', logs);
        // Filtere nur Logs für den aktuellen Wochentag
        this.todayLogs = logs.filter(log => this.isCurrentDayLog(log));
      },
      error: (error) => {
        console.error('Fehler beim Laden der heutigen Logs:', error);
      }
    });
  }

  getMedicationsForSlot(slot: TimeSlot): MedicationLog[] {
    // Filtere zuerst alle Logs für dieses Zeitfenster
    const logsInSlot = this.todayLogs.filter(log => {
      if (!log.taken_time) return false;
      const logTime = this.parseTime(log.taken_time);
      const startTime = this.parseTime(slot.startTime);
      const endTime = this.parseTime(slot.endTime);
      return logTime >= startTime && logTime <= endTime;
    });

    // Gruppiere nach medication_name und behalte nur den aktuellsten Eintrag
    const latestLogs = new Map<string, MedicationLog>();
    logsInSlot.forEach(log => {
      const existing = latestLogs.get(log.medication_name);
      if (!existing || new Date(log.timestamp) > new Date(existing.timestamp)) {
        latestLogs.set(log.medication_name, log);
      }
    });

    return Array.from(latestLogs.values());
  }

  isMedicationTaken(medication: MedicationLog, slot: TimeSlot): boolean {
    if (!medication.timestamp) return false;
    const now = new Date();
    const today = new Date(now.getTime() - (now.getTimezoneOffset() * 60000)).toISOString().split('T')[0];
    const logDate = new Date(medication.timestamp).toISOString().split('T')[0];
    return today === logDate;
  }

  isCurrentDayLog(log: MedicationLog): boolean {
    const currentDay = this.getCurrentDay();
    // Vergleiche case-insensitive und berücksichtige mögliche Leerzeichen
    return log.medication_name.toLowerCase().trim() === currentDay.toLowerCase().trim();
  }

  getCurrentDay(): string {
    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    const now = new Date();
    // Berücksichtige die lokale Zeitzone
    const localDate = new Date(now.getTime() - (now.getTimezoneOffset() * 60000));
    return days[localDate.getDay()];
  }

  getCurrentDayName(): string {
    const dayNames = {
      'Monday': 'Montag',
      'Tuesday': 'Dienstag',
      'Wednesday': 'Mittwoch',
      'Thursday': 'Donnerstag',
      'Friday': 'Freitag',
      'Saturday': 'Samstag',
      'Sunday': 'Sonntag'
    };
    return dayNames[this.getCurrentDay() as keyof typeof dayNames];
  }

  parseTime(timeStr: string): number {
    const [hours, minutes] = timeStr.split(':').map(Number);
    return hours * 60 + minutes;
  }

  formatTime(timeStr: string | undefined): string {
    return timeStr ? timeStr.substring(0, 5) : '';
  }
} 