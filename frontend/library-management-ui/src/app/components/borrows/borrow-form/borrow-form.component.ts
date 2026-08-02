import { Component, OnInit } from '@angular/core';
import { BorrowService } from '../../../services/borrow.service';
import { CreateBorrowRequest } from '../../../models/borrow.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-borrow-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './borrow-form.component.html',
  styleUrl: './borrow-form.component.scss'
})
export class BorrowFormComponent implements OnInit {
  loading = false;
  submitting = false;
  errorMessage = '';
  form = { bookCopyId: '', memberId: '' };
  errors: Record<string, string> = {};
  toastMessages: ToastMessage[] = [];

  constructor(private borrowService: BorrowService, private router: Router) {}

  onSubmit(): void {
    this.errors = {};
    if (!this.form.bookCopyId.trim()) this.errors.bookCopyId = 'Book copy is required';
    if (!this.form.memberId.trim()) this.errors.memberId = 'Member is required';
    if (Object.keys(this.errors).length > 0) return;

    this.submitting = true;
    this.borrowService.create({
      bookCopyId: this.form.bookCopyId,
      memberId: this.form.memberId
    }).subscribe({
      next: (response) => {
        this.submitting = false;
        if (response.success && response.data) {
          this.showToast('success', `Book "${response.data.bookTitle}" borrowed successfully.`);
          this.router.navigate(['/borrows']);
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to create borrow';
          this.showToast('error', this.errorMessage);
        }
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = err.message || 'Failed to create borrow';
        this.showToast('error', this.errorMessage);
      }
    });
  }
  onCancel(): void { this.router.navigate(['/borrows']); }
  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(this.toastMessages, type, message);
  }
  onDismissToast(id: string): void { this.toastMessages = this.toastMessages.filter((t) => t.id !== id); }
}
