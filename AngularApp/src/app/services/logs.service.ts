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

@Injectable({ providedIn: 'root' })
export class LogsService {
  private apiUrl = `${environment.apiUrl}/logs`;

  constructor(private http: HttpClient) {}

  getLogs(success?: boolean): Observable<Log[]> {
    const url = success !== undefined ? `${this.apiUrl}?success=${success}` : this.apiUrl;
    return this.http.get<Log[]>(url);
  }
}