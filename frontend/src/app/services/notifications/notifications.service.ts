import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { NotificationListDto } from '../../models/notification/notification-list.dto';
import { AuthService } from '../../services/auth/authservice';

@Injectable({ providedIn: 'root' })
export class NotificationsService {
  constructor(private http: HttpClient, private auth: AuthService) {}

  private baseForCurrentRole(): string {
    const roles = this.auth.getUserRoles() || [];
    if (roles.includes('Admin')) return `${environment.apiBaseUrl}/api/admin`;
    return `${environment.apiBaseUrl}/api/customer`;
  }

  getNotifications(): Observable<NotificationListDto[]> {
    const BASE = this.baseForCurrentRole();
    return this.http.get<NotificationListDto[]>(`${BASE}/notifications`);
  }

  markAsRead(notificationId: number) {
    const BASE = this.baseForCurrentRole();
    return this.http.put(`${BASE}/notifications/${notificationId}/read`, null, { responseType: 'text' });
  }

  markAsUnread(notificationId: number) {
    const BASE = this.baseForCurrentRole();
    return this.http.put(`${BASE}/notifications/${notificationId}/unread`, null, { responseType: 'text' });
  }
}
