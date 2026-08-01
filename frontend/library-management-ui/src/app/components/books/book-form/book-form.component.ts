import { Component, OnInit } from '@angular/core';
import { BookService } from '../../../services/book.service';
import { CreateBookRequest, UpdateBookRequest } from '../../../models/book.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './book-form.component.html',
  styleUrl: './book-form.component.scss'
})
export class BookFormComponent implements OnInit {
  isEdit = false;
  bookId: string | null = null;
  loading = false;
  submitting = false;
  errorMessage = '';

  form = {
    title: '',
    isbn: '',
    description: '',
    publisher: '',
    publishedYear: null as number | null,
    language: '',
    imageUrl: '',
    authorId: '',
    categoryId: ''
  };

  errors: Record<string, string> = {};

  toastMessages: ToastMessage[] = [];

  constructor(
    private bookService: BookService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = history.state?.bookId;
    if (id) {
      this.isEdit = true;
      this.bookId = id;
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
          const book = response.data;
          this.form = {
            title: book.title,
            isbn: book.isbn,
            description: book.description || '',
            publisher: book.publisher || '',
            publishedYear: book.publishedYear,
            language: book.language || '',
            imageUrl: book.imageUrl || '',
            authorId: book.authorId,
            categoryId: book.categoryId
          };
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

  onSubmit(): void {
    this.errors = {};
    this.submitting = true;

    // Basic validation
    if (!this.form.title.trim()) {
      this.errors.title = 'Title is required';
    }
    if (!this.form.isbn.trim()) {
      this.errors.isbn = 'ISBN is required';
    }
    if (!this.form.authorId) {
      this.errors.authorId = 'Author is required';
    }
    if (!this.form.categoryId) {
      this.errors.categoryId = 'Category is required';
    }

    if (Object.keys(this.errors).length > 0) {
      this.submitting = false;
      return;
    }

    const request = {
      title: this.form.title.trim(),
      isbn: this.form.isbn.trim(),
      description: this.form.description.trim() || null,
      publisher: this.form.publisher.trim() || null,
      publishedYear: this.form.publishedYear,
      language: this.form.language.trim() || null,
      imageUrl: this.form.imageUrl.trim() || null,
      authorId: this.form.authorId,
      categoryId: this.form.categoryId
    };

    if (this.isEdit && this.bookId) {
      this.bookService.update(this.bookId, request as UpdateBookRequest).subscribe({
        next: (response) => {
          this.submitting = false;
          if (response.success) {
            this.showToast('success', `Book "${this.form.title}" updated successfully.`);
            this.router.navigate(['/books']);
          } else {
            this.errorMessage = response.errors?.join(' ') || 'Failed to update book';
            this.showToast('error', this.errorMessage);
          }
        },
        error: (err) => {
          this.submitting = false;
          this.errorMessage = err.message || 'Failed to update book';
          this.showToast('error', this.errorMessage);
        }
      });
    } else {
      this.bookService.create(request as CreateBookRequest).subscribe({
        next: (response) => {
          this.submitting = false;
          if (response.success) {
            this.showToast('success', `Book "${this.form.title}" created successfully.`);
            this.router.navigate(['/books']);
          } else {
            this.errorMessage = response.errors?.join(' ') || 'Failed to create book';
            this.showToast('error', this.errorMessage);
          }
        },
        error: (err) => {
          this.submitting = false;
          this.errorMessage = err.message || 'Failed to create book';
          this.showToast('error', this.errorMessage);
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/books']);
  }

  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(this.toastMessages, type, message);
  }

  onDismissToast(id: string): void {
    this.toastMessages = this.toastMessages.filter((t) => t.id !== id);
  }
}
