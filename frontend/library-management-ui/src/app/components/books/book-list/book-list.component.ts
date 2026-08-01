import { Component, OnInit } from '@angular/core';
import { BookService } from '../../../services/book.service';
import { BookDto } from '../../../models/book.models';
import { PaginatedResult } from '../../../models/result.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './book-list.component.html',
  styleUrl: './book-list.component.scss'
})
export class BookListComponent implements OnInit {
  books: BookDto[] = [];
  pagination: PaginatedResult<BookDto> | null = null;
  loading = false;
  errorMessage = '';
  searchQuery = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;

  toastMessages: ToastMessage[] = [];

  constructor(
    private bookService: BookService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.loading = true;
    this.errorMessage = '';

    this.bookService.search(
      this.searchQuery || undefined,
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.pagination = response.data;
          this.books = response.data.items;
          this.totalPages = Math.ceil(response.data.totalCount / this.pageSize);
        } else {
          this.books = [];
          this.totalPages = 0;
          this.errorMessage = response.errors?.join(' ') || 'Failed to load books';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load books';
        this.showToast('error', this.errorMessage);
      }
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadBooks();
  }

  onClearSearch(): void {
    this.searchQuery = '';
    this.currentPage = 1;
    this.loadBooks();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadBooks();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadBooks();
  }

  onDelete(book: BookDto): void {
    if (!confirm(`Are you sure you want to delete "${book.title}"? This action cannot be undone.`)) {
      return;
    }

    this.bookService.delete(book.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToast('success', `Book "${book.title}" deleted successfully.`);
          this.loadBooks();
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to delete book';
          this.showToast('error', this.errorMessage);
        }
      },
      error: (err) => {
        this.errorMessage = err.message || 'Failed to delete book';
        this.showToast('error', this.errorMessage);
      }
    });
  }

  getStatusBadge(availableCopies: number): string {
    return availableCopies > 0 ? 'badge-available' : 'badge-unavailable';
  }

  getStatusText(availableCopies: number): string {
    return availableCopies > 0 ? `${availableCopies} available` : 'Unavailable';
  }

  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(this.toastMessages, type, message);
  }

  onDismissToast(id: string): void {
    this.toastMessages = this.toastMessages.filter((t) => t.id !== id);
  }
}
