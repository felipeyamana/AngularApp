import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private themeKey = 'app-theme';

  getTheme(): 'light' | 'dark' {
    return (localStorage.getItem(this.themeKey) as 'light' | 'dark') || 'light';
  }

  setTheme(theme: 'light' | 'dark'): void {
    localStorage.setItem(this.themeKey, theme);
  }
}