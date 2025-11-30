import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';

import { provideRouter } from '@angular/router';
 
import { routes } from './app.routes';
 
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { authInterceptor } from './interceptors/auth.interceptor';
 
export const appConfig: ApplicationConfig = {

  providers: [

    provideBrowserGlobalErrorListeners(),

    provideZoneChangeDetection({ eventCoalescing: true }),
 
    // Routing

    provideRouter(routes),
 
    // Include HTTP + Interceptors

    provideHttpClient(

      withInterceptors([authInterceptor])

    )

  ]

};

 