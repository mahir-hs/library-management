import { Component, OnInit } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { BorrowHistoryDto, OverdueBookDto, MemberActivityDto, ReportSummaryDto } from '../../../models/report.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-reports-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ToastComponent, SpinnerComponent],
  templateUrl: './reports-page.component.html',
  styleUrl: './reports-page.component.scss'
})
export class ReportsPageComponent implements OnInit {
  activeTab = 'borrow-history';
  loading = false;
  errorMessage = '';

  // Borrow History
  borrowHistory: BorrowHistoryDto[] = [];
  startDate = '';
  endDate = '';

  // Overdue
  overdueBooks: OverdueBookDto[] = [];

  // Member Activity
  memberActivity: MemberActivityDto[] = [];
  memberSearchId = '';
  memberSummary: ReportSummaryDto | null = null;

  toastMessages: ToastMessage[] = [];

  constructor(private reportService: ReportService) {}

  ngOnInit(): void {
    this.loadBorrowHistory();
    this.loadOverdue();
  }

  loadBorrowHistory(): void {
    this.loading = true;
    this.errorMessage = '';
    this.reportService.getBorrowHistory(this.startDate || undefined, this.endDate || undefined).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.borrowHistory = response.data;
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to load borrow history';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load borrow history';
      }
    });
  }

  loadOverdue(): void {
    this.reportService.getOverdue().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.overdueBooks = response.data;
        }
      }
    });
  }

  loadMemberActivity(): void {
    if (!this.memberSearchId.trim()) return;
    this.loading = true;
    this.errorMessage = '';
    this.reportService.getMemberActivity(this.memberSearchId.trim()).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.memberActivity = [response.data];
          this.memberSummary = {
            totalBooks: 0,
            activeBorrows: response.data.activeBorrows,
            pendingReservations: 0,
            overdueCount: response.data.overdueBorrows
          };
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to load member activity';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load member activity';
      }
    });
  }

  onSearchBorrowHistory(): void {
    this.loadBorrowHistory();
  }

  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(this.toastMessages, type, message);
  }
  onDismissToast(id: string): void {
    this.toastMessages = this.toastMessages.filter((t) => t.id !== id);
  }
}
