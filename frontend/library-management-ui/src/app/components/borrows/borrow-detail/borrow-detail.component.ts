import { Component, OnInit } from '@angular/core';
import { BorrowService } from '../../../services/borrow.service';
import { BorrowDto } from '../../../models/borrow.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-borrow-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './borrow-detail.component.html',
  styleUrl: './borrow-detail.component.scss'
})
export class BorrowDetailComponent implements OnInit {
  borrow: BorrowDto | null = null;
  loading = false;
  errorMessage = '';
  toastMessages: ToastMessage[] = [];

  constructor(private borrowService: BorrowService, private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadBorrow(id);
  }
  loadBorrow(id: string): void {
    this.loading = true;
    this.borrowService.getById(id).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.borrow = response.data;
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Borrow not found';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load borrow';
      }
    });
  }
  onReturn(): void {
    if (!this.borrow || !confirm(`Return "${this.borrow.bookTitle}"?`)) return;
    this.borrowService.returnBorrow({ borrowId: this.borrow.id }).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToast('success', 'Book returned successfully.');
          this.loadBorrow(this.borrow!.id);
        } else {
          this.showToast('error', response.errors?.join(' ') || 'Failed to return');
        }
      },
      error: (err) => {
        this.showToast('error', err.message || 'Failed to return');
      }
    });
  }
  onBack(): void { this.router.navigate(['/borrows']); }
  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
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
  onDismissToast(id: string): void { this.toastMessages = this.toastMessages.filter((t) => t.id !== id); }
}
