import { Component, OnInit } from '@angular/core';
import { SettingsService } from '../settings.service';

@Component({
  selector: 'app-theme-ui',
  templateUrl: './theme-ui.component.html',
  styleUrls: ['./theme-ui.component.scss'],
  standalone: true
})
export class ThemeUiComponent implements OnInit {
  isDarkMode = false;

  constructor(private settings: SettingsService) {}

  ngOnInit(): void {
    this.isDarkMode = this.settings.getTheme() === 'dark';
    this.applyTheme();
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    this.settings.setTheme(this.isDarkMode ? 'dark' : 'light');
    this.applyTheme();
  }

  private applyTheme() {
    document.body.setAttribute('data-bs-theme', this.isDarkMode ? 'dark' : 'light');
  }
}
