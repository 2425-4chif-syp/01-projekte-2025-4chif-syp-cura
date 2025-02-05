import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Drug, MedicationPlan } from '../models/drug.interface';

@Injectable({
  providedIn: 'root'
})
export class MedicationService {
  private apiUrl = 'http://localhost:3000/api'; // Geändert auf den Express-Server-Port

  constructor(private http: HttpClient) { }

  // Medikamente
  getDrugs(): Observable<Drug[]> {
    return this.http.get<Drug[]>(`${this.apiUrl}/drugs`);
  }

  getDrug(id: number): Observable<Drug> {
    return this.http.get<Drug>(`${this.apiUrl}/drugs/${id}`);
  }

  createDrug(drug: Drug): Observable<Drug> {
    return this.http.post<Drug>(`${this.apiUrl}/drugs`, drug);
  }

  updateDrug(id: number, drug: Drug): Observable<Drug> {
    return this.http.put<Drug>(`${this.apiUrl}/drugs/${id}`, drug);
  }

  deleteDrug(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/drugs/${id}`);
  }

  // Medikamentenplan
  getMedicationPlan(): Observable<MedicationPlan[]> {
    return this.http.get<MedicationPlan[]>(`${this.apiUrl}/plan`);
  }

  addToPlan(plan: MedicationPlan): Observable<MedicationPlan> {
    return this.http.post<MedicationPlan>(`${this.apiUrl}/plan`, plan);
  }

  updatePlanEntry(id: number, plan: MedicationPlan): Observable<MedicationPlan> {
    return this.http.put<MedicationPlan>(`${this.apiUrl}/plan/${id}`, plan);
  }

  deletePlanEntry(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/plan/${id}`);
  }
} 