import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeUiComponent } from './theme-ui/theme-ui.component';
import { Navbar } from '../components/navbar/navbar';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, Navbar, ThemeUiComponent],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent {
  activeMenu = 'theme-ui';
}