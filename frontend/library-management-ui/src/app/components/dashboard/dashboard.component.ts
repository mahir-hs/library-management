import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReportService } from '../../services/report.service';
import { BorrowService } from '../../services/borrow.service';
import { ReservationService } from '../../services/reservation.service';
import { AuthService } from '../../services/auth.service';
import { ReportSummaryDto } from '../../models/report.models';
import { MyBorrowsResponse } from '../../models/borrow.models';
import { MyReservationsResponse } from '../../models/reservation.models';
import { PaginatedResult, Result } from '../../models/result.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  summary: ReportSummaryDto | null = null;
  loading = false;
  errorMessage = '';

  // Member-specific data
  myBorrows: MyBorrowsResponse[] = [];
  myReservations: MyReservationsResponse[] = [];
  memberLoading = false;
  memberError = '';

  constructor(
    private reportService: ReportService,
    private borrowService: BorrowService,
    private reservationService: ReservationService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';

    if (this.isMember) {
      this.loadMemberData();
    } else {
      this.loadAdminData();
    }
  }

  private clearLoading(): void {
    this.loading = false;
    this.memberLoading = false;
  }

  loadAdminData(): void {
    this.reportService.getSummary().subscribe({
      next: (response) => {
        this.clearLoading();
        if (response.success && response.data) {
          this.summary = response.data;
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to load summary';
        }
      },
      error: (err) => {
        this.clearLoading();
        this.errorMessage = err.message || 'Failed to load summary';
      }
    });
  }

  loadMemberData(): void {
    this.memberLoading = true;
    this.memberError = '';

    this.borrowService.getMine(1, 5).subscribe({
      next: (response) => {
        this.memberLoading = false;
        if (response.success && response.data) {
          this.myBorrows = response.data.items;
        } else {
          this.myBorrows = [];
          this.memberError = response.errors?.join(' ') || 'Failed to load borrows';
        }
      },
      error: (err) => {
        this.memberLoading = false;
        this.memberError = err.message || 'Failed to load borrows';
      }
    });

    this.reservationService.getAll(1, 5).subscribe({
      next: (response) => {
        this.memberLoading = false;
        if (response.success && response.data) {
          this.myReservations = response.data.items;
        } else {
          this.myReservations = [];
        }
      },
      error: () => {
        this.memberLoading = false;
        this.myReservations = [];
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

  get activeBorrowsCount(): number {
    return this.myBorrows.filter(b => b.status === 'Borrowed' || b.status === 'Overdue').length;
  }

  get overdueCount(): number {
    return this.myBorrows.filter(b => b.status === 'Overdue').length;
  }

  get pendingReservationsCount(): number {
    return this.myReservations.filter(r => r.status === 'Pending').length;
  }
}
