import { Component, OnInit } from '@angular/core';
import { BranchService } from '../../../services/branch.service';
import { BranchDto } from '../../../models/branch.models';
import { PaginatedResult } from '../../../models/result.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-branch-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './branch-list.component.html',
  styleUrl: './branch-list.component.scss'
})
export class BranchListComponent implements OnInit {
  branches: BranchDto[] = [];
  pagination: PaginatedResult<BranchDto> | null = null;
  loading = false;
  errorMessage = '';
  searchName = '';
  searchCode = '';
  searchIsActive: boolean | undefined = undefined;
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;

  toastMessages: ToastMessage[] = [];

  constructor(
    private branchService: BranchService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadBranches();
  }

  loadBranches(): void {
    this.loading = true;
    this.errorMessage = '';

    this.branchService.search(
      this.searchName || undefined,
      this.searchCode || undefined,
      this.searchIsActive,
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.pagination = response.data;
          this.branches = response.data.items;
          this.totalPages = Math.ceil(response.data.totalCount / this.pageSize);
        } else {
          this.branches = [];
          this.totalPages = 0;
          this.errorMessage = response.errors?.join(' ') || 'Failed to load branches';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load branches';
        this.showToast('error', this.errorMessage);
      }
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadBranches();
  }

  onClearSearch(): void {
    this.searchName = '';
    this.searchCode = '';
    this.searchIsActive = undefined;
    this.currentPage = 1;
    this.loadBranches();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadBranches();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadBranches();
  }

  onDelete(branch: BranchDto): void {
    if (!confirm(`Are you sure you want to delete branch "${branch.name}"? This action cannot be undone.`)) {
      return;
    }

    this.branchService.delete(branch.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToast('success', `Branch "${branch.name}" deleted successfully.`);
          this.loadBranches();
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to delete branch';
          this.showToast('error', this.errorMessage);
        }
      },
      error: (err) => {
        this.errorMessage = err.message || 'Failed to delete branch';
        this.showToast('error', this.errorMessage);
      }
    });
  }

  onToggleActive(branch: BranchDto): void {
    const updated = { ...branch, isActive: !branch.isActive };
    this.branchService.update(branch.id, updated).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToast('success', `Branch "${branch.name}" ${branch.isActive ? 'deactivated' : 'activated'} successfully.`);
          this.loadBranches();
        } else {
          this.showToast('error', response.errors?.join(' ') || 'Failed to update branch');
        }
      },
      error: (err) => {
        this.showToast('error', err.message || 'Failed to update branch');
      }
    });
  }

  getStatusBadge(isActive: boolean): string {
    return isActive ? 'badge-active' : 'badge-inactive';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }

  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(this.toastMessages, type, message);
  }

  onDismissToast(id: string): void {
    this.toastMessages = this.toastMessages.filter((t) => t.id !== id);
  }
}
