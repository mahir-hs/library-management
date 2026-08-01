import { Component, OnInit } from '@angular/core';
import { ReservationService } from '../../../services/reservation.service';
import { ReservationDto } from '../../../models/reservation.models';
import { PaginatedResult } from '../../../models/result.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-reservation-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './reservation-list.component.html',
  styleUrl: './reservation-list.component.scss'
})
export class ReservationListComponent implements OnInit {
  reservations: ReservationDto[] = [];
  pagination: PaginatedResult<ReservationDto> | null = null;
  loading = false;
  errorMessage = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  toastMessages: ToastMessage[] = [];

  constructor(private reservationService: ReservationService, private router: Router) {}

  ngOnInit(): void {
    this.loadReservations();
  }

  loadReservations(): void {
    this.loading = true;
    this.errorMessage = '';
    this.reservationService.getAll(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.pagination = response.data;
          this.reservations = response.data.items;
          this.totalPages = Math.ceil(response.data.totalCount / this.pageSize);
        } else {
          this.reservations = [];
          this.totalPages = 0;
          this.errorMessage = response.errors?.join(' ') || 'Failed to load reservations';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load reservations';
        this.showToast('error', this.errorMessage);
      }
    });
  }

  onFulfill(reservation: ReservationDto): void {
    if (!confirm(`Fulfill reservation for "${reservation.bookTitle}"?`)) return;
    this.reservationService.fulfill(reservation.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToast('success', `Reservation fulfilled for "${reservation.bookTitle}".`);
          this.loadReservations();
        } else {
          this.showToast('error', response.errors?.join(' ') || 'Failed to fulfill');
        }
      },
      error: (err) => {
        this.showToast('error', err.message || 'Failed to fulfill reservation');
      }
    });
  }

  onCancel(reservation: ReservationDto): void {
    if (!confirm(`Cancel reservation for "${reservation.bookTitle}"?`)) return;
    this.reservationService.cancel({ reservationId: reservation.id }).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToast('success', `Reservation cancelled for "${reservation.bookTitle}".`);
          this.loadReservations();
        } else {
          this.showToast('error', response.errors?.join(' ') || 'Failed to cancel');
        }
      },
      error: (err) => {
        this.showToast('error', err.message || 'Failed to cancel reservation');
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadReservations();
  }

  getStatusBadge(status: string): string {
    switch (status?.toLowerCase()) {
      case 'pending': return 'badge-pending';
      case 'fulfilled': return 'badge-fulfilled';
      case 'cancelled': return 'badge-cancelled';
      case 'expired': return 'badge-expired';
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
