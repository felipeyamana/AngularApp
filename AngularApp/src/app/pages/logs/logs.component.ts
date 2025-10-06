import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LogsService, Log, PagedLogsResponse } from '../../services/logs.service';
import { Navbar } from '../../components/navbar/navbar';
import { NgxPaginationModule } from 'ngx-pagination';

@Component({
  selector: 'app-logs',
  standalone: true,
  imports: [CommonModule, Navbar, NgxPaginationModule],
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.scss']
})
export class LogsComponent implements OnInit {
  logs: Log[] = [];
  loading = false;
  filterSuccess?: boolean;

  // pagination
  page = 1;
  totalItems = 0;
  readonly itemsPerPage = 20; // backend will still enforce the 20 max logs per page rule

  Math = Math;

  constructor(private logsService: LogsService) {}

  ngOnInit(): void {
    this.loadLogs();
  }

  loadLogs(): void {
    this.loading = true;

    this.logsService.getLogs(this.page, this.filterSuccess).subscribe({
      next: (response) => {
        if (response && 'items' in response) {
          this.logs = response.items;
          this.totalItems = response.totalCount ?? response.items.length;
        } 
        else {
          this.logs = [];
          this.totalItems = 0;
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load logs', err);
        this.loading = false;
      }
    });
  }

  toggleSuccessFilter(success?: boolean) {
    this.filterSuccess = success;
    this.page = 1; // reset to first page when changing filter
    this.loadLogs();
  }

  onPageChange(newPage: number) {
    this.page = newPage;
    this.loadLogs();
  }
}