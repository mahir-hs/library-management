import { Component, OnInit } from '@angular/core';
import { BookService } from '../../../services/book.service';
import { BookDto } from '../../../models/book.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-book-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './book-detail.component.html',
  styleUrl: './book-detail.component.scss'
})
export class BookDetailComponent implements OnInit {
  book: BookDto | null = null;
  loading = false;
  errorMessage = '';

  toastMessages: ToastMessage[] = [];

  constructor(
    private bookService: BookService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadBook(id);
    }
  }

  loadBook(id: string): void {
    this.loading = true;
    this.errorMessage = '';

    this.bookService.getById(id).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.book = response.data;
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to load book';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load book';
      }
    });
  }

  onEdit(): void {
    if (this.book) {
      this.router.navigate(['/books', this.book.id, 'edit'], {
        state: { bookId: this.book.id }
      });
    }
  }

  onBack(): void {
    this.router.navigate(['/books']);
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
