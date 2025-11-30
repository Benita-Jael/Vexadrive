import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { VehicleListDto } from '../../models/vehicle/VehicleListDto';
import { VehicleCreateDto } from '../../models/vehicle/VehicleCreateDto';
import { VehicleDetailsDto } from '../../models/vehicle/VehicleDetailsDto';
import { VehicleUpdateDto } from '../../models/vehicle/VehicleUpdateDto';
import { OwnerListDto } from '../../models/owners/OwnerListDto';
import { environment } from '../../../environments/environment';

const VEHICLE_API_BASE_URL = `${environment.apiBaseUrl}/api/vehicle`;
const OWNER_API_BASE_URL = `${environment.apiBaseUrl}/api/owner`;

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  constructor(private http: HttpClient) {}

  // Get all vehicles
  getAllVehicles(): Observable<VehicleListDto[]> {
    return this.http.get<VehicleListDto[]>(`${VEHICLE_API_BASE_URL}/all`);
  }

  // Delete a vehicle
  deleteVehicle(vehicleId: number): Observable<any> {
    return this.http.delete(`${VEHICLE_API_BASE_URL}/delete/${vehicleId}`, { responseType: 'text' });
  }

  // Create a vehicle
  // Backend returns a plain message string on success; keep return type flexible
  createVehicle(model: VehicleCreateDto): Observable<any> {
    return this.http.post<any>(`${VEHICLE_API_BASE_URL}/add`, model);
  }

  // Search vehicles
  searchVehicles(id?: number, model?: string, numberPlate?: string, type?: string, color?: string, ownerId?: string): Observable<VehicleDetailsDto[]> {
    const params: any = {};
    if (id != null) params.id = id.toString();
    if (model) params.model = model;
    if (numberPlate) params.numberPlate = numberPlate;
    if (type) params.type = type;
    if (color) params.color = color;
    if (ownerId != null) params.customerUserId = ownerId;

    return this.http.get<VehicleDetailsDto[]>(`${VEHICLE_API_BASE_URL}/search`, { params });
  }

  // Get all owners (for dropdowns)
  getOwners(): Observable<OwnerListDto[]> {
    return this.http.get<OwnerListDto[]>(`${OWNER_API_BASE_URL}/AllOwners`);
  }

  // Get vehicle by ID
  getVehicleById(vehicleId: number): Observable<VehicleDetailsDto> {
    return this.http.get<VehicleDetailsDto>(`${VEHICLE_API_BASE_URL}/${vehicleId}`);
  }

  // Update a vehicle
  updateVehicle(model: VehicleUpdateDto): Observable<any> {
    return this.http.put(`${VEHICLE_API_BASE_URL}/update/${model.vehicleId}`, model, { responseType: 'text' });
  }
}
