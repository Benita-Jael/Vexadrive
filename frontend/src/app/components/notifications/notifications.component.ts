import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationsService } from '../../services/notifications/notifications.service';
import { AuthService } from '../../services/auth/authservice';
import { NotificationListDto } from '../../models/notification/notification-list.dto';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { interval } from 'rxjs';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit, OnDestroy {
  notifications: NotificationListDto[] = [];
  filteredNotifications: NotificationListDto[] = [];
  loading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  private destroy$ = new Subject<void>();

  filterStatus = 'all'; // 'all', 'unread', 'read'
  userRoles: string[] = [];

  constructor(private notificationsService: NotificationsService, private authService: AuthService) {
    this.userRoles = this.authService.getUserRoles() || [];
  }

  ngOnInit() {
    this.load();
    
    // Poll for new notifications every 10 seconds
    interval(10000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.load());
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  load() {
    this.loading = true;
    this.notificationsService.getNotifications().subscribe({
      next: (data) => {
        this.notifications = data;
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load notifications', err);
        this.loading = false;
        this.errorMessage = 'Failed to load notifications';
      }
    });
  }

  applyFilters() {
    let filtered = this.notifications;
    
    // Only show "raised a new request" notifications if user is Admin
    if (this.userRoles.includes('Admin')) {
      filtered = filtered.filter(n =>
        n.title && n.title.toLowerCase().includes('raised a new request')
      );
    }
    
    // Apply read/unread filter
    if (this.filterStatus === 'unread') {
      filtered = filtered.filter(n => !n.isRead);
    } else if (this.filterStatus === 'read') {
      filtered = filtered.filter(n => n.isRead);
    }
    
    this.filteredNotifications = filtered;
  }

  markRead(n: NotificationListDto) {
    this.notificationsService.markAsRead(n.notificationId).subscribe({
      next: () => {
        this.successMessage = 'Notification marked as read';
        this.load();
        setTimeout(() => this.successMessage = null, 3000);
      },
      error: (err) => {
        console.error('Failed to mark read', err);
        this.errorMessage = 'Failed to mark notification as read';
      }
    });
  }

  markUnread(n: NotificationListDto) {
    this.notificationsService.markAsUnread(n.notificationId).subscribe({
      next: () => {
        this.successMessage = 'Notification marked as unread';
        this.load();
        setTimeout(() => this.successMessage = null, 3000);
      },
      error: (err) => {
        console.error('Failed to mark unread', err);
        this.errorMessage = 'Failed to mark notification as unread';
      }
    });
  }

  markAllRead() {
    const unreadIds = this.notifications.filter(n => !n.isRead).map(n => n.notificationId);
    if (unreadIds.length === 0) {
      this.errorMessage = 'All notifications are already marked as read';
      return;
    }

    let completed = 0;
    unreadIds.forEach(id => {
      this.notificationsService.markAsRead(id).subscribe({
        next: () => {
          completed++;
          if (completed === unreadIds.length) {
            this.successMessage = `Marked ${unreadIds.length} notification(s) as read`;
            this.load();
            setTimeout(() => this.successMessage = null, 3000);
          }
        },
        error: (err) => console.error('Failed to mark read', err)
      });
    });
  }

  getNotificationIcon(notificationId: number): string {
    // Simple cycling through icons based on ID
    const icons = ['🔔', '📧', '✅', '⚠️', '📝'];
    return icons[notificationId % icons.length];
  }

  getNotificationTypeBadge(notificationId: number): string {
    // Simple cycling through badge types based on ID
    const badges = ['bg-info', 'bg-success', 'bg-warning', 'bg-danger', 'bg-secondary'];
    return badges[notificationId % badges.length];
  }

  getUnreadCount(): number {
    return this.notifications.filter(n => !n.isRead).length;
  }
}
