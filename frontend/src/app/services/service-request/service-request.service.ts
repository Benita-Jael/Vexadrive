import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ServiceRequestCreateDTO } from '../../models/service-request/service-request-create.dto';
import { ServiceRequestDetailsDTO } from '../../models/service-request/service-request-details.dto';
import { ServiceStatus } from '../../models/service-request/service-status.enum';

@Injectable({ providedIn: 'root' })
export class ServiceRequestService {
  private base = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  createRequest(dto: ServiceRequestCreateDTO): Observable<ServiceRequestDetailsDTO> {
    return this.http.post<ServiceRequestDetailsDTO>(`${this.base}/api/customer/requests`, dto);
  }

  getCustomerRequests(): Observable<ServiceRequestDetailsDTO[]> {
    return this.http.get<ServiceRequestDetailsDTO[]>(`${this.base}/api/customer/requests`);
  }

  getCustomerVehicles(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/api/customer/vehicles`);
  }

  getRequestById(id: number): Observable<ServiceRequestDetailsDTO> {
    return this.http.get<ServiceRequestDetailsDTO>(`${this.base}/api/customer/requests/${id}`);
  }

  // Admin flows
  getAllRequests(): Observable<ServiceRequestDetailsDTO[]> {
    return this.http.get<ServiceRequestDetailsDTO[]>(`${this.base}/api/admin/requests`);
  }

  updateStatus(id: number, status: ServiceStatus) {
    return this.http.put<ServiceRequestDetailsDTO>(`${this.base}/api/admin/requests/${id}/status`, status);
  }

  // backend uses ETD (estimated delivery date)
  updateEtd(id: number, etd: string) {
    return this.http.put<ServiceRequestDetailsDTO>(`${this.base}/api/admin/requests/${id}/etd`, etd);
  }

  deleteRequest(id: number) {
    return this.http.delete(`${this.base}/api/admin/requests/${id}`, { responseType: 'text' });
  }

  // Upload bill (admin) - multipart/form-data
  uploadBill(id: number, file: File) {
    const fd = new FormData();
    fd.append('file', file, file.name);
    return this.http.post(`${this.base}/api/admin/requests/${id}/bill`, fd);
  }

  // Download bill (customer) - returns blob
  downloadBill(id: number) {
    return this.http.get(`${this.base}/api/customer/requests/${id}/bill`, { responseType: 'blob' });
  }

  searchRequests(vehicleType?: string, plateNumber?: string, customerEmail?: string): Observable<any[]> {
    let url = `${this.base}/api/admin/requests/search?`;
    const params = [];
    if (vehicleType) params.push(`vehicleType=${encodeURIComponent(vehicleType)}`);
    if (plateNumber) params.push(`plateNumber=${encodeURIComponent(plateNumber)}`);
    if (customerEmail) params.push(`customerEmail=${encodeURIComponent(customerEmail)}`);
    url += params.join('&');
    return this.http.get<any[]>(url);
  }
}
