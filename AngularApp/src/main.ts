import 'zone.js';
import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { routes } from './app/app.routes';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';

document.body.setAttribute('data-bs-theme', localStorage.getItem('app-theme') || 'light');

bootstrapApplication(App, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withFetch()) 
  ]
}).catch(err => console.error(err));
