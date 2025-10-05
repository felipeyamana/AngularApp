import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SettingsService } from '../settings.service';

@Component({
  selector: 'app-theme-ui',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './theme-ui.component.html',
  styleUrls: ['./theme-ui.component.scss']
})
export class ThemeUiComponent implements OnInit {
  isDarkMode = false;
  layoutDensity: 'comfortable' | 'compact' = 'comfortable';

  constructor(private settings: SettingsService) {}

  ngOnInit(): void {
    // Load saved preferences
    const prefs = this.settings.getPreferences();
    this.isDarkMode = prefs.theme === 'dark';
    this.layoutDensity = prefs.layout || 'comfortable';

    // Apply them on startup
    this.applyTheme();
    this.applyDensity();
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    const newTheme = this.isDarkMode ? 'dark' : 'light';
    this.settings.updatePreferences({ theme: newTheme });
    this.applyTheme();
  }

  setDensity(): void {
    this.settings.updatePreferences({ layout: this.layoutDensity });
    this.applyDensity();
  }

  private applyTheme(): void {
    document.body.setAttribute('data-bs-theme', this.isDarkMode ? 'dark' : 'light');
  }

  private applyDensity(): void {
    document.body.classList.toggle('compact', this.layoutDensity === 'compact');
  }
}
