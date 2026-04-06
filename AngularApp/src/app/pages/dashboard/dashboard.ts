import { Component } from '@angular/core';
import { Navbar } from '../../components/navbar/navbar';
import { DashboardChartCardComponent } from './dashboard-chart-card/dashboard-chart-card.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [Navbar, DashboardChartCardComponent],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {}
