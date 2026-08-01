import { Component, OnInit } from '@angular/core';
import { BranchService } from '../../../services/branch.service';
import { BranchDto } from '../../../models/branch.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-branch-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './branch-detail.component.html',
  styleUrl: './branch-detail.component.scss'
})
export class BranchDetailComponent implements OnInit {
  branch: BranchDto | null = null;
  loading = false;
  errorMessage = '';

  toastMessages: ToastMessage[] = [];

  constructor(
    private branchService: BranchService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadBranch(id);
    }
  }

  loadBranch(id: string): void {
    this.loading = true;
    this.errorMessage = '';

    this.branchService.getById(id).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.branch = response.data;
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Branch not found';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load branch';
      }
    });
  }

  onEdit(): void {
    if (this.branch) {
      this.router.navigate(['/branches', this.branch.id, 'edit'], {
        state: { branchId: this.branch.id }
      });
    }
  }

  onBack(): void {
    this.router.navigate(['/branches']);
  }

  getStatusBadge(isActive: boolean): string {
    return isActive ? 'badge-active' : 'badge-inactive';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
