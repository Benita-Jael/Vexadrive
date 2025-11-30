import { Routes } from '@angular/router';

import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';
 
import { HomeComponent } from './components/home/home.component';
import { SigninComponent } from './components/auth/signin/signin.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { AboutComponent } from './components/about/about.component';
import { ContactComponent } from './components/contact/contact.component';
import { ServiceRequestsComponent } from './components/service-request/service-requests.component';
import { AdminRequestsComponent } from './components/admin/admin-requests.component';
import { AdminAnalyticsComponent } from './components/admin/admin-analytics.component';
import { NotificationsComponent } from './components/notifications/notifications.component';
 
export const routes: Routes = [
  { path: '', redirectTo: '/signin', pathMatch: 'full' },
  { path: 'signin', component: SigninComponent },
  { path: 'register', component: RegisterComponent },
 
  { path: 'home', component: HomeComponent, canActivate: [authGuard] },
  { path: 'services', component: ServiceRequestsComponent, canActivate: [authGuard] },
  { path: 'notifications', component: NotificationsComponent, canActivate: [authGuard] },
  { path: 'about', component: AboutComponent },
  { path: 'contact', component: ContactComponent },
 
  // Admin routes
  { path: 'admin/requests', component: AdminRequestsComponent, canActivate: [roleGuard], data: { roles: ['Admin'] } },
  { path: 'admin/analytics', component: AdminAnalyticsComponent, canActivate: [roleGuard], data: { roles: ['Admin'] } },
 
  // Wildcard
  { path: '**', redirectTo: '/signin' }
];

 