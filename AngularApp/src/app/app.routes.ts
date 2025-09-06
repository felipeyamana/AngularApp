import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Dashboard } from './pages/dashboard/dashboard';
import { authGuard } from './guards/auth.guard';
import { Unauthorized } from './pages/unauthorized/unauthorized';

export const routes: Routes = [
  { path: 'login', component: Login },
  { 
    path: 'dashboard', 
    component: Dashboard, 
    canActivate: [authGuard],
    data: { roles: ['User'] }
  },
  { path: 'unauthorized', component: Unauthorized },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
