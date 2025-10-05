import 'zone.js';
import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { routes } from './app/app.routes';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';

// Load user preferences once at startup
const prefs = JSON.parse(localStorage.getItem('user-preferences') || '{}');

// Apply theme, light by default
document.body.setAttribute('data-bs-theme', prefs.theme || 'light');

// Apply layout density
if (prefs.layout === 'compact') {
  document.body.classList.add('compact');
} else {
  document.body.classList.remove('compact');
}

bootstrapApplication(App, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withFetch())
  ]
}).catch(err => console.error(err));