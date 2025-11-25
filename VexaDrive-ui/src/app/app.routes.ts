import { Routes } from '@angular/router';

import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';
 
import { HomeComponent } from './components/home/home.component';

import { SigninComponent } from './components/auth/signin/signin.component';

import { AboutComponent } from './components/about/about.component';

import { ContactComponent } from './components/contact/contact.component';
 
export const routes: Routes = [

  { path: '', redirectTo: '/signin', pathMatch: 'full' },
 
  { path: 'home', component: HomeComponent, canActivate: [authGuard] },

  {
    path: 'vehicles',
    loadComponent: () =>
      import('./components/vehicle/vehicle.component').then(v => v.VehicleComponent),
    canActivate: [authGuard]
  },

  {
    path: 'owners',
    loadComponent: () =>
      import('./components/owner/owner.component').then(o => o.OwnerComponent),
    canActivate: [roleGuard],
    data: { roles: ['Admin'] }
  },
 
  { path: 'about', component: AboutComponent },

  { path: 'contact', component: ContactComponent },
 
  { path: 'signin', component: SigninComponent },
 
  // No registration in VexaDrive → redirect

  { path: 'register', redirectTo: '/signin' },
 
  // Wildcard

  { path: '**', redirectTo: '/signin' }

];

 