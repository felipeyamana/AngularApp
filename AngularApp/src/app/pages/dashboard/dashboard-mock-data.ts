export type DashboardPeriod = 'month' | 'sixMonths' | 'year';

export type DashboardChartKey = 'users' | 'chat' | 'logins' | 'logs';

export const DASHBOARD_PERIOD_OPTIONS: ReadonlyArray<{ value: DashboardPeriod; label: string }> = [
  { value: 'month', label: 'Last month' },
  { value: 'sixMonths', label: 'Last 6 months' },
  { value: 'year', label: 'Last year' },
];

export interface DashboardSeries {
  labels: string[];
  values: number[];
  /** Optional KPI line under the title */
  summary?: string;
}

function mulberry32(seed: number): () => number {
  return () => {
    let t = (seed += 0x6d2b79f5);
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

function pointCount(period: DashboardPeriod): number {
  switch (period) {
    case 'month':
      return 30;
    case 'sixMonths':
      return 26;
    case 'year':
      return 12;
  }
}

export function axisLabels(period: DashboardPeriod): string[] {
  const n = pointCount(period);
  if (period === 'year') {
    return ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  }
  if (period === 'month') {
    return Array.from({ length: n }, (_, i) => `${i + 1}`);
  }
  return Array.from({ length: n }, (_, i) => `W${i + 1}`);
}

/** Cumulative user registrations (upward trend with noise). */
function seriesUsers(period: DashboardPeriod): DashboardSeries {
  const labels = axisLabels(period);
  const rng = mulberry32(period.length + 11_427);
  let total = 620 + Math.floor(rng() * 180);
  const values = labels.map(() => {
    total += Math.floor(rng() * 22) - 5;
    return total;
  });
  return {
    labels,
    values,
    summary: `Total users: ${values[values.length - 1].toLocaleString()}`,
  };
}

/** Chat messages with spikes and quiet periods. */
function seriesChat(period: DashboardPeriod): DashboardSeries {
  const labels = axisLabels(period);
  const rng = mulberry32(period.length + 90_001);
  let prev = 180 + Math.floor(rng() * 120);
  const values = labels.map((_, i) => {
    const drift = (rng() - 0.5) * 80;
    let v = Math.max(20, Math.round(prev + drift));
    if (rng() > 0.92) {
      v = Math.round(v * (2.1 + rng()));
    }
    if (rng() > 0.94) {
      v = Math.round(v * (0.25 + rng() * 0.2));
    }
    prev = v;
    return v;
  });
  const peak = Math.max(...values);
  const low = Math.min(...values);
  return {
    labels,
    values,
    summary: `Peak ${peak.toLocaleString()} msgs · Low ${low.toLocaleString()} msgs`,
  };
}

/** Login sessions per bucket (daily / weekly / monthly depending on period). */
function seriesLogins(period: DashboardPeriod): DashboardSeries {
  const labels = axisLabels(period);
  const rng = mulberry32(period.length + 3_331);
  const base = period === 'month' ? 55 : period === 'sixMonths' ? 320 : 2100;
  const values = labels.map(() => {
    const jitter = (rng() - 0.5) * base * 0.35;
    return Math.max(8, Math.round(base + jitter));
  });
  const avg = Math.round(values.reduce((a, b) => a + b, 0) / values.length);
  const unit = period === 'month' ? 'day' : period === 'sixMonths' ? 'week' : 'month';
  return {
    labels,
    values,
    summary: `Avg ${avg.toLocaleString()} sessions / ${unit}`,
  };
}

/** Log entries written per bucket. */
function seriesLogs(period: DashboardPeriod): DashboardSeries {
  const labels = axisLabels(period);
  const rng = mulberry32(period.length + 77_707);
  const scale = period === 'month' ? 1 : period === 'sixMonths' ? 6.5 : 28;
  const values = labels.map(() => {
    const core = (400 + rng() * 500) * scale;
    const spike = rng() > 0.88 ? rng() * core * 1.4 : 0;
    return Math.round(core + spike);
  });
  const total = values.reduce((a, b) => a + b, 0);
  return {
    labels,
    values,
    summary: `Total log entries: ${total.toLocaleString()}`,
  };
}

export function getDashboardSeries(key: DashboardChartKey, period: DashboardPeriod): DashboardSeries {
  switch (key) {
    case 'users':
      return seriesUsers(period);
    case 'chat':
      return seriesChat(period);
    case 'logins':
      return seriesLogins(period);
    case 'logs':
      return seriesLogs(period);
  }
}

export function datasetLabelFor(key: DashboardChartKey): string {
  switch (key) {
    case 'users':
      return 'Registered users (cumulative)';
    case 'chat':
      return 'Chat messages';
    case 'logins':
      return 'Login sessions';
    case 'logs':
      return 'Log entries';
  }
}
