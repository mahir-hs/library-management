import { Component, OnInit } from '@angular/core';
import { BranchService } from '../../../services/branch.service';
import { CreateBranchRequest, UpdateBranchRequest } from '../../../models/branch.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-branch-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './branch-form.component.html',
  styleUrl: './branch-form.component.scss'
})
export class BranchFormComponent implements OnInit {
  isEdit = false;
  branchId: string | null = null;
  loading = false;
  submitting = false;
  errorMessage = '';

  form = {
    name: '',
    code: '',
    address: '',
    phone: '',
    email: ''
  };

  errors: Record<string, string> = {};

  toastMessages: ToastMessage[] = [];

  constructor(
    private branchService: BranchService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = history.state?.branchId;
    if (id) {
      this.isEdit = true;
      this.branchId = id;
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
          const branch = response.data;
          this.form = {
            name: branch.name,
            code: branch.code,
            address: branch.address,
            phone: branch.phone || '',
            email: branch.email || ''
          };
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to load branch';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load branch';
      }
    });
  }

  onSubmit(): void {
    this.errors = {};
    this.submitting = true;

    // Basic validation
    if (!this.form.name.trim()) {
      this.errors.name = 'Name is required';
    }
    if (!this.form.code.trim()) {
      this.errors.code = 'Code is required';
    }
    if (!this.form.address.trim()) {
      this.errors.address = 'Address is required';
    }

    if (Object.keys(this.errors).length > 0) {
      this.submitting = false;
      return;
    }

    const request = {
      name: this.form.name.trim(),
      code: this.form.code.trim(),
      address: this.form.address.trim(),
      phone: this.form.phone.trim() || null,
      email: this.form.email.trim() || null
    };

    if (this.isEdit && this.branchId) {
      this.branchService.update(this.branchId, request as UpdateBranchRequest).subscribe({
        next: (response) => {
          this.submitting = false;
          if (response.success) {
            this.showToast('success', `Branch "${this.form.name}" updated successfully.`);
            this.router.navigate(['/branches']);
          } else {
            this.errorMessage = response.errors?.join(' ') || 'Failed to update branch';
            this.showToast('error', this.errorMessage);
          }
        },
        error: (err) => {
          this.submitting = false;
          this.errorMessage = err.message || 'Failed to update branch';
          this.showToast('error', this.errorMessage);
        }
      });
    } else {
      this.branchService.create(request as CreateBranchRequest).subscribe({
        next: (response) => {
          this.submitting = false;
          if (response.success) {
            this.showToast('success', `Branch "${this.form.name}" created successfully.`);
            this.router.navigate(['/branches']);
          } else {
            this.errorMessage = response.errors?.join(' ') || 'Failed to create branch';
            this.showToast('error', this.errorMessage);
          }
        },
        error: (err) => {
          this.submitting = false;
          this.errorMessage = err.message || 'Failed to create branch';
          this.showToast('error', this.errorMessage);
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/branches']);
  }

  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(this.toastMessages, type, message);
  }

  onDismissToast(id: string): void {
    this.toastMessages = this.toastMessages.filter((t) => t.id !== id);
  }
}
