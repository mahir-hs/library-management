import { Component, OnInit } from '@angular/core';
import { BorrowService } from '../../../services/borrow.service';
import { BorrowDto } from '../../../models/borrow.models';
import { PaginatedResult } from '../../../models/result.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-borrow-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './borrow-list.component.html',
  styleUrl: './borrow-list.component.scss'
})
export class BorrowListComponent implements OnInit {
  borrows: BorrowDto[] = [];
  pagination: PaginatedResult<BorrowDto> | null = null;
  loading = false;
  errorMessage = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  showOverdueOnly = false;

  toastMessages: ToastMessage[] = [];

  constructor(private borrowService: BorrowService, private router: Router) {}

  ngOnInit(): void {
    this.loadBorrows();
  }

  loadBorrows(): void {
    this.loading = true;
    this.errorMessage = '';

    const obs = this.showOverdueOnly
      ? this.borrowService.getOverdue(this.currentPage, this.pageSize)
      : this.borrowService.getMine(this.currentPage, this.pageSize);

    obs.subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.pagination = response.data;
          this.borrows = response.data.items;
          this.totalPages = Math.ceil(response.data.totalCount / this.pageSize);
        } else {
          this.borrows = [];
          this.totalPages = 0;
          this.errorMessage = response.errors?.join(' ') || 'Failed to load borrows';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load borrows';
        this.showToast('error', this.errorMessage);
      }
    });
  }

  onReturn(borrow: BorrowDto): void {
    if (!confirm(`Return "${borrow.bookTitle}"?`)) return;

    this.borrowService.returnBorrow({ borrowId: borrow.id }).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToast('success', `Book "${borrow.bookTitle}" returned successfully.`);
          this.loadBorrows();
        } else {
          this.showToast('error', response.errors?.join(' ') || 'Failed to return book');
        }
      },
      error: (err) => {
        this.showToast('error', err.message || 'Failed to return book');
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadBorrows();
  }

  onToggleOverdue(): void {
    this.showOverdueOnly = !this.showOverdueOnly;
    this.currentPage = 1;
    this.loadBorrows();
  }

  getStatusBadge(status: string): string {
    switch (status?.toLowerCase()) {
      case 'active': return 'badge-active';
      case 'returned': return 'badge-returned';
      case 'overdue': return 'badge-overdue';
      default: return 'badge-default';
    }
  }

  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(this.toastMessages, type, message);
  }
  onDismissToast(id: string): void {
    this.toastMessages = this.toastMessages.filter((t) => t.id !== id);
  }
}
