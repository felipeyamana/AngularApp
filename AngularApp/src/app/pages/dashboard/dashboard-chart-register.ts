import {
  BarController,
  BarElement,
  CategoryScale,
  Chart,
  Filler,
  Legend,
  LinearScale,
  LineController,
  LineElement,
  PointElement,
  Title,
  Tooltip,
} from 'chart.js';

let registered = false;

export function ensureChartJsRegistered(): void {
  if (registered) {
    return;
  }
  Chart.register(
    LineController,
    LineElement,
    PointElement,
    LinearScale,
    CategoryScale,
    BarController,
    BarElement,
    Title,
    Tooltip,
    Legend,
    Filler
  );
  registered = true;
}
