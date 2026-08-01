import { Component, OnInit } from '@angular/core';
import { MemberService } from '../../../services/member.service';
import { MemberDto } from '../../../models/member.models';
import { PaginatedResult } from '../../../models/result.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.scss'
})
export class MemberListComponent implements OnInit {
  members: MemberDto[] = [];
  pagination: PaginatedResult<MemberDto> | null = null;
  loading = false;
  errorMessage = '';
  searchQuery = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;

  toastMessages: ToastMessage[] = [];

  constructor(
    private memberService: MemberService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(): void {
    this.loading = true;
    this.errorMessage = '';

    this.memberService.search(
      this.searchQuery || undefined,
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.pagination = response.data;
          this.members = response.data.items;
          this.totalPages = Math.ceil(response.data.totalCount / this.pageSize);
        } else {
          this.members = [];
          this.totalPages = 0;
          this.errorMessage = response.errors?.join(' ') || 'Failed to load members';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load members';
        this.showToast('error', this.errorMessage);
      }
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadMembers();
  }

  onClearSearch(): void {
    this.searchQuery = '';
    this.currentPage = 1;
    this.loadMembers();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadMembers();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadMembers();
  }

  onDelete(member: MemberDto): void {
    if (!confirm(`Are you sure you want to delete member "${member.fullName}"?`)) {
      return;
    }

    this.memberService.delete(member.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToast('success', `Member "${member.fullName}" deleted successfully.`);
          this.loadMembers();
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to delete member';
          this.showToast('error', this.errorMessage);
        }
      },
      error: (err) => {
        this.errorMessage = err.message || 'Failed to delete member';
        this.showToast('error', this.errorMessage);
      }
    });
  }

  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(this.toastMessages, type, message);
  }

  onDismissToast(id: string): void {
    this.toastMessages = this.toastMessages.filter((t) => t.id !== id);
  }
}
