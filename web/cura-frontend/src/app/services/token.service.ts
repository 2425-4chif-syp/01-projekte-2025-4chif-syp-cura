import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';

/**
 * Service to extract user information from Keycloak JWT token
 * This service provides helper methods to access custom claims like patient_id, caregiver_id, etc.
 */
@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor(private keycloak: KeycloakService) {}

  /**
   * Get the patient_id from the JWT token
   * Returns null if user is not a patient or patient_id claim is missing
   */
  async getPatientId(): Promise<number | null> {
    try {
      const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
      
      if (!tokenParsed) {
        console.warn('Token not parsed yet');
        return null;
      }

      // @ts-ignore - patient_id is a custom claim
      const patientId = tokenParsed.patient_id;
      
      if (!patientId) {
        return null;
      }

      return parseInt(patientId, 10);
    } catch (error) {
      console.error('Error getting patient ID from token:', error);
      return null;
    }
  }

  /**
   * Get the caregiver_id from the JWT token
   * Returns null if user is not a caregiver or caregiver_id claim is missing
   */
  async getCaregiverId(): Promise<number | null> {
    try {
      const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
      
      if (!tokenParsed) {
        return null;
      }

      // @ts-ignore - caregiver_id is a custom claim
      const caregiverId = tokenParsed.caregiver_id;
      
      if (!caregiverId) {
        return null;
      }

      return parseInt(caregiverId, 10);
    } catch (error) {
      console.error('Error getting caregiver ID from token:', error);
      return null;
    }
  }

  /**
   * Get the location_id from the JWT token
   * Returns null if location_id claim is missing
   */
  async getLocationId(): Promise<number | null> {
    try {
      const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
      
      if (!tokenParsed) {
        return null;
      }

      // @ts-ignore - location_id is a custom claim
      const locationId = tokenParsed.location_id;
      
      if (!locationId) {
        return null;
      }

      return parseInt(locationId, 10);
    } catch (error) {
      console.error('Error getting location ID from token:', error);
      return null;
    }
  }

  /**
   * Get all roles from the JWT token
   */
  async getRoles(): Promise<string[]> {
    try {
      const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
      
      if (!tokenParsed) {
        return [];
      }

      // @ts-ignore - realm_roles is a custom claim
      const realmRoles = tokenParsed.realm_roles || [];
      
      return realmRoles.filter((role: string) => 
        role === 'patient' || role === 'caregiver' || role === 'admin'
      );
    } catch (error) {
      console.error('Error getting roles from token:', error);
      return [];
    }
  }

  /**
   * Check if user has a specific role
   */
  async hasRole(role: 'patient' | 'caregiver' | 'admin'): Promise<boolean> {
    const roles = await this.getRoles();
    return roles.includes(role);
  }

  /**
   * Get the username from the token
   */
  async getUsername(): Promise<string | null> {
    try {
      const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
      
      if (!tokenParsed) {
        return null;
      }

      // @ts-ignore
      return tokenParsed.preferred_username || tokenParsed.name || null;
    } catch (error) {
      console.error('Error getting username from token:', error);
      return null;
    }
  }

  /**
   * Get the email from the token
   */
  async getEmail(): Promise<string | null> {
    try {
      const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
      
      if (!tokenParsed) {
        return null;
      }

      // @ts-ignore
      return tokenParsed.email || null;
    } catch (error) {
      console.error('Error getting email from token:', error);
      return null;
    }
  }

  /**
   * Get the full name from the token
   */
  async getFullName(): Promise<string | null> {
    try {
      const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
      
      if (!tokenParsed) {
        return null;
      }

      // @ts-ignore
      const firstName = tokenParsed.given_name || '';
      // @ts-ignore
      const lastName = tokenParsed.family_name || '';
      
      if (firstName && lastName) {
        return `${firstName} ${lastName}`;
      }

      // @ts-ignore
      return tokenParsed.name || null;
    } catch (error) {
      console.error('Error getting full name from token:', error);
      return null;
    }
  }

  /**
   * Debug: Print all token claims to console
   */
  async debugPrintToken(): Promise<void> {
    try {
      const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
      console.log('=== JWT Token Claims ===');
      console.log(JSON.stringify(tokenParsed, null, 2));
      console.log('========================');
    } catch (error) {
      console.error('Error printing token:', error);
    }
  }
}
