import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Patient {
  id: number;
  name: string;
  age?: number;
  locationId?: number;
  phoneNumber?: string;
  email?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getPatientById(patientId: number): Observable<Patient> {
    return this.http.get<Patient>(`${this.API_URL}/Patients/${patientId}`);
  }

  getAllPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>(`${this.API_URL}/Patients`);
  }
}
