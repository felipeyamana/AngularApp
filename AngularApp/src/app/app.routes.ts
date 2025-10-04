import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Dashboard } from './pages/dashboard/dashboard';
import { UserProfileComponent } from './pages/user-profile/user-profile.component';
import { authGuard } from './guards/auth.guard';
import { loginGuard } from './guards/login.guard';
import { Unauthorized } from './pages/unauthorized/unauthorized';
import { SettingsComponent } from './settings/settings.component';

export const routes: Routes = [
  { path: 'login', component: Login, canActivate:[loginGuard] },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { 
    path: 'dashboard', 
    component: Dashboard, 
    canActivate: [authGuard],
    data: { roles: ['User'] }
  },
  { 
    path: 'user-profile', 
    component: UserProfileComponent, 
    canActivate: [authGuard],
    data: { roles: ['User'] }
  },
  { path: 'settings', component: SettingsComponent },
  { path: 'unauthorized', component: Unauthorized }
];
