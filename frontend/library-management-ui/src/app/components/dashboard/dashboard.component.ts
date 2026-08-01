import { Component, OnInit } from '@angular/core';
import { ReportService } from '../../services/report.service';
import { AuthService } from '../../services/auth.service';
import { ReportSummaryDto } from '../../models/report.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  summary: ReportSummaryDto | null = null;
  loading = false;
  errorMessage = '';

  constructor(
    private reportService: ReportService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadSummary();
  }

  loadSummary(): void {
    this.loading = true;
    this.errorMessage = '';

    this.reportService.getSummary().subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.summary = response.data;
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to load summary';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load summary';
      }
    });
  }

  get isAdmin(): boolean {
    return this.authService.getRole() === 'Admin';
  }

  get isLibrarian(): boolean {
    return this.authService.getRole() === 'Librarian';
  }

  get isMember(): boolean {
    return this.authService.getRole() === 'Member';
  }
}
