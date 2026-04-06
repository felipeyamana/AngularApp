import { isPlatformBrowser } from '@angular/common';
import {
  Component,
  ElementRef,
  Injector,
  Input,
  OnDestroy,
  PLATFORM_ID,
  ViewChild,
  afterNextRender,
  inject,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Chart, ChartConfiguration } from 'chart.js';
import {
  DASHBOARD_PERIOD_OPTIONS,
  DashboardChartKey,
  DashboardPeriod,
  datasetLabelFor,
  getDashboardSeries,
} from '../dashboard-mock-data';
import { ensureChartJsRegistered } from '../dashboard-chart-register';

@Component({
  selector: 'app-dashboard-chart-card',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './dashboard-chart-card.component.html',
  styleUrl: './dashboard-chart-card.component.scss',
})
export class DashboardChartCardComponent implements OnDestroy {
  @Input({ required: true }) title!: string;
  @Input({ required: true }) chartKey!: DashboardChartKey;
  @Input({ required: true }) chartType!: 'line' | 'bar';

  @ViewChild('canvasEl') private canvasRef?: ElementRef<HTMLCanvasElement>;

  protected readonly periodOptions = DASHBOARD_PERIOD_OPTIONS;
  protected period: DashboardPeriod = 'month';

  protected summary = '';

  private readonly platformId = inject(PLATFORM_ID);
  private readonly injector = inject(Injector);
  private chart?: Chart;

  protected get isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  constructor() {
    afterNextRender(
      () => {
        if (!this.isBrowser) {
          return;
        }
        ensureChartJsRegistered();
        this.drawChart();
      },
      { injector: this.injector }
    );
  }

  ngOnDestroy(): void {
    this.chart?.destroy();
  }

  protected onPeriodChange(): void {
    if (!this.isBrowser) {
      return;
    }
    this.drawChart();
  }

  private drawChart(): void {
    const canvas = this.canvasRef?.nativeElement;
    if (!canvas) {
      return;
    }

    const series = getDashboardSeries(this.chartKey, this.period);
    this.summary = series.summary ?? '';

    const label = datasetLabelFor(this.chartKey);
    const { border, background } = colorsFor(this.chartKey);

    this.chart?.destroy();

    const config: ChartConfiguration = {
      type: this.chartType,
      data: {
        labels: series.labels,
        datasets: [
          {
            label,
            data: series.values,
            borderColor: border,
            backgroundColor: background,
            borderWidth: this.chartType === 'line' ? 2 : 0,
            fill: this.chartType === 'line',
            tension: this.chartType === 'line' ? 0.25 : undefined,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        interaction: { mode: 'index', intersect: false },
        plugins: {
          legend: {
            display: true,
            position: 'bottom',
            labels: { boxWidth: 12 },
          },
        },
        scales: {
          x: {
            grid: { display: false },
            ticks: { maxRotation: 0, autoSkip: true, maxTicksLimit: 12 },
          },
          y: {
            beginAtZero: true,
            ticks: { precision: 0 },
          },
        },
      },
    };

    this.chart = new Chart(canvas, config);
  }
}

function colorsFor(key: DashboardChartKey): { border: string; background: string } {
  switch (key) {
    case 'users':
      return { border: 'rgb(13, 110, 253)', background: 'rgba(13, 110, 253, 0.12)' };
    case 'chat':
      return { border: 'rgb(220, 53, 69)', background: 'rgba(220, 53, 69, 0.1)' };
    case 'logins':
      return { border: 'rgb(253, 126, 20)', background: 'rgba(253, 126, 20, 0.35)' };
    case 'logs':
      return { border: 'rgb(25, 135, 84)', background: 'rgba(25, 135, 84, 0.35)' };
  }
}
