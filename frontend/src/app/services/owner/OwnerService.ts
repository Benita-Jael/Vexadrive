import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { OwnerListDto } from '../../models/owners/OwnerListDto';
import { OwnerCreateDto } from '../../models/owners/OwnerCreateDto';
import { OwnerDetailsDto } from '../../models/owners/OwnerDetailsDto';
import { OwnerUpdateDto } from '../../models/owners/OwnerUpdateDto';
import { environment } from '../../../environments/environment';

const OWNER_API_BASE_URL = `${environment.apiBaseUrl}/api/Owner`;

@Injectable({
  providedIn: 'root'
})
export class OwnerService {
  constructor(private http: HttpClient) {}

  // Get all owners
  getAllOwners(): Observable<OwnerListDto[]> {
    return this.http.get<OwnerListDto[]>(`${OWNER_API_BASE_URL}/AllOwners`);
  }

  // Delete an owner
  deleteOwner(ownerId: string): Observable<any> {
    return this.http.delete(`${OWNER_API_BASE_URL}/deleteOwner/${ownerId}`, { responseType: 'text' });
  }

  // Create an owner
  createOwner(model: OwnerCreateDto): Observable<OwnerDetailsDto> {
    return this.http.post<OwnerDetailsDto>(`${OWNER_API_BASE_URL}/AddOwner`, model);
  }

  // Search owners
  searchOwners(id?: string, firstName?: string, lastName?: string, email?: string, contact?: string): Observable<OwnerDetailsDto[]> {
    const params: any = {};
    if (id != null) params.id = id;
    if (firstName) params.firstName = firstName;
    if (lastName) params.lastName = lastName;
    if (email) params.email = email;
    if (contact) params.contact = contact;

    return this.http.get<OwnerDetailsDto[]>(`${OWNER_API_BASE_URL}/searchOwner`, { params });
  }

  // Get owner by ID
  getOwnerById(ownerId: string): Observable<OwnerDetailsDto> {
    return this.http.get<OwnerDetailsDto>(`${OWNER_API_BASE_URL}/GetOwnerById/${ownerId}`);
  }

  // Update an owner
  updateOwner(model: OwnerUpdateDto): Observable<any> {
    return this.http.put(`${OWNER_API_BASE_URL}/UpdateOwner/${model.ownerId}`, model, { responseType: 'text' });
  }
}
