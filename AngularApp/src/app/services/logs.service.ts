import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Log {
  id: number;
  logTypeName: string;
  userFullName?: string;
  action: string;
  description?: string;
  ipAddress?: string;
  eventDate: string;
  actionSuccess?: boolean;
}

export interface PagedLogsResponse {
  page: number;
  pageSize: number;
  totalCount: number;
  items: Log[];
}

@Injectable({ providedIn: 'root' })
export class LogsService {
  private apiUrl = `${environment.apiUrl}/logs`;

  constructor(private http: HttpClient) {}

  getLogs(page = 1, success?: boolean): Observable<PagedLogsResponse | Log[]> {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    if (success !== undefined) params.append('success', String(success));

    const url = `${this.apiUrl}?${params.toString()}`;
    return this.http.get<PagedLogsResponse | Log[]>(url);
  }
}