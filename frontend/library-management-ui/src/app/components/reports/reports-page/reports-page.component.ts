import { Component, OnInit } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { AuthService } from '../../../services/auth.service';
import {
  BorrowHistoryDto,
  OverdueBookDto,
  MemberActivityDto,
  ReportSummaryDto,
} from '../../../models/report.models';
import { PaginatedResult } from '../../../models/result.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ToastComponent,
  ToastMessage,
} from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-reports-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ToastComponent, SpinnerComponent],
  templateUrl: './reports-page.component.html',
  styleUrl: './reports-page.component.scss',
})
export class ReportsPageComponent implements OnInit {
  activeTab = 'borrow-history';
  loading = false;
  errorMessage = '';

  // Summary
  summary: ReportSummaryDto | null = null;

  // Borrow History
  borrowHistory: BorrowHistoryDto[] = [];
  borrowHistoryPagination: PaginatedResult<BorrowHistoryDto> | null = null;
  startDate = '';
  endDate = '';
  borrowHistoryPage = 1;
  borrowHistoryPageSize = 10;
  borrowHistoryTotalPages = 0;

  // Overdue
  overdueBooks: OverdueBookDto[] = [];
  overduePagination: PaginatedResult<OverdueBookDto> | null = null;
  overduePage = 1;
  overduePageSize = 10;
  overdueTotalPages = 0;

  // Member Activity
  memberActivity: MemberActivityDto[] = [];
  memberSearchName = '';
  memberSearchId = '';

  toastMessages: ToastMessage[] = [];

  constructor(
    private reportService: ReportService,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.loadSummary();
    this.loadBorrowHistory();
    this.loadOverdue();
  }

  loadSummary(): void {
    this.reportService.getSummary().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.summary = response.data;
        } else {
          this.errorMessage =
            response.errors?.join(' ') || 'Failed to load summary';
        }
      },
      error: (err) => {
        this.errorMessage = err.message || 'Failed to load summary';
      },
    });
  }

  loadBorrowHistory(): void {
    this.loading = true;
    this.errorMessage = '';
    this.reportService
      .getBorrowHistory(
        this.borrowHistoryPage,
        this.borrowHistoryPageSize,
        this.startDate || undefined,
        this.endDate || undefined,
      )
      .subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success && response.data) {
            this.borrowHistoryPagination = response.data;
            this.borrowHistory = response.data.items;
            this.borrowHistoryTotalPages = Math.ceil(
              response.data.totalCount / this.borrowHistoryPageSize,
            );
          } else {
            this.borrowHistory = [];
            this.borrowHistoryTotalPages = 0;
            this.errorMessage =
              response.errors?.join(' ') || 'Failed to load borrow history';
          }
        },
        error: (err) => {
          this.loading = false;
          this.errorMessage = err.message || 'Failed to load borrow history';
        },
      });
  }

  loadOverdue(): void {
    this.reportService
      .getOverdue(this.overduePage, this.overduePageSize)
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.overduePagination = response.data;
            this.overdueBooks = response.data.items;
            this.overdueTotalPages = Math.ceil(
              response.data.totalCount / this.overduePageSize,
            );
          } else {
            this.overdueBooks = [];
            this.overdueTotalPages = 0;
            this.errorMessage =
              response.errors?.join(' ') || 'Failed to load overdue books';
          }
        },
      });
  }

  loadMemberActivity(): void {
    const id = this.memberSearchId.trim();
    if (!id) return;
    this.loading = true;
    this.errorMessage = '';
    this.reportService.getMemberActivity(id).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.memberActivity = response.data;
        } else {
          this.errorMessage =
            response.errors?.join(' ') || 'Failed to load member activity';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load member activity';
      },
    });
  }

  onSearchBorrowHistory(): void {
    this.borrowHistoryPage = 1;
    this.loadBorrowHistory();
  }

  onBorrowHistoryPageChange(page: number): void {
    this.borrowHistoryPage = page;
    this.loadBorrowHistory();
  }

  onOverduePageChange(page: number): void {
    this.overduePage = page;
    this.loadOverdue();
  }

  onSearchMemberActivity(): void {
    this.loadMemberActivity();
  }

  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(
      this.toastMessages,
      type,
      message,
    );
  }
  onDismissToast(id: string): void {
    this.toastMessages = this.toastMessages.filter((t) => t.id !== id);
  }
}
