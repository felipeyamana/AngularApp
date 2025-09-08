import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Dashboard } from './pages/dashboard/dashboard';
import { authGuard } from './guards/auth.guard';
import { loginGuard } from './guards/login.guard';
import { Unauthorized } from './pages/unauthorized/unauthorized';

export const routes: Routes = [
  { path: 'login', component: Login, canActivate:[loginGuard] },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { 
    path: 'dashboard', 
    component: Dashboard, 
    canActivate: [authGuard],
    data: { roles: ['User'] }
  },
  { path: 'unauthorized', component: Unauthorized }
];
