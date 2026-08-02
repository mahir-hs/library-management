import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { UserDto } from '../../../models/auth.models';
import { PaginatedResult } from '../../../models/result.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  ToastComponent,
  ToastMessage,
} from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    ToastComponent,
    SpinnerComponent,
  ],
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.scss',
})
export class UsersListComponent implements OnInit {
  users: UserDto[] = [];
  pagination: PaginatedResult<UserDto> | null = null;
  loading = false;
  errorMessage = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;

  toastMessages: ToastMessage[] = [];

  constructor(
    private userService: UserService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.errorMessage = '';

    this.userService.getAll(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.pagination = response.data;
          this.users = response.data.items;
          this.totalPages = Math.ceil(response.data.totalCount / this.pageSize);
        } else {
          this.users = [];
          this.totalPages = 0;
          this.errorMessage =
            response.errors?.join(' ') || 'Failed to load users';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load users';
        this.showToast('error', this.errorMessage);
      },
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadUsers();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadUsers();
  }

  onDelete(user: UserDto): void {
    if (
      !confirm(
        `Are you sure you want to delete user "${user.username}"? This action cannot be undone.`,
      )
    ) {
      return;
    }

    this.userService.delete(user.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToast(
            'success',
            `User "${user.username}" deleted successfully.`,
          );
          this.loadUsers();
        } else {
          this.showToast(
            'error',
            response.errors?.join(' ') || 'Failed to delete user',
          );
        }
      },
      error: (err) => {
        this.showToast('error', err.message || 'Failed to delete user');
      },
    });
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
