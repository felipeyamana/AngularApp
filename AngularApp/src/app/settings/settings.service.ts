import { Injectable } from '@angular/core';

interface UserPreferences {
  theme: 'light' | 'dark';
  layout: 'comfortable' | 'compact';
}

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private readonly key = 'user-preferences';

  getPreferences(): UserPreferences {
    return JSON.parse(localStorage.getItem(this.key) || '{}');
  }

  updatePreferences(update: Partial<UserPreferences>): void {
    const current = this.getPreferences();
    const merged = { ...current, ...update };
    localStorage.setItem(this.key, JSON.stringify(merged));
  }
}