export interface DailyStatus {
  date: string;
  scheduled: number;
  taken: number;
  status: 'checked' | 'partial' | 'missed' | 'empty';
}
