import { Component, OnInit } from '@angular/core';
import { MemberService } from '../../../services/member.service';
import { CreateMemberRequest, UpdateMemberRequest } from '../../../models/member.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-member-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './member-form.component.html',
  styleUrl: './member-form.component.scss'
})
export class MemberFormComponent implements OnInit {
  isEdit = false;
  memberId: string | null = null;
  loading = false;
  submitting = false;
  errorMessage = '';

  form = {
    userId: '',
    membershipNumber: '',
    address: '',
    phoneNumber: ''
  };

  errors: Record<string, string> = {};
  toastMessages: ToastMessage[] = [];

  constructor(private memberService: MemberService, private router: Router) {}

  ngOnInit(): void {
    const id = history.state?.memberId;
    if (id) {
      this.isEdit = true;
      this.memberId = id;
      this.loadMember(id);
    }
  }

  loadMember(id: string): void {
    this.loading = true;
    this.memberService.getById(id).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          const m = response.data;
          this.form = {
            userId: m.userId,
            membershipNumber: m.membershipNumber,
            address: m.address,
            phoneNumber: m.phoneNumber || ''
          };
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Failed to load member';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load member';
      }
    });
  }

  onSubmit(): void {
    this.errors = {};
    this.submitting = true;

    if (!this.form.membershipNumber.trim()) this.errors.membershipNumber = 'Membership number is required';
    if (!this.form.address.trim()) this.errors.address = 'Address is required';
    if (Object.keys(this.errors).length > 0) { this.submitting = false; return; }

    const request = {
      userId: this.form.userId,
      membershipNumber: this.form.membershipNumber.trim(),
      address: this.form.address.trim(),
      phoneNumber: this.form.phoneNumber.trim() || null
    };

    const obs = this.isEdit && this.memberId
      ? this.memberService.update(this.memberId, request as UpdateMemberRequest)
      : this.memberService.create(request as CreateMemberRequest);

    obs.subscribe({
      next: (response) => {
        this.submitting = false;
        if (response.success) {
          this.showToast('success', `Member "${this.form.membershipNumber}" ${this.isEdit ? 'updated' : 'created'} successfully.`);
          this.router.navigate(['/members']);
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Operation failed';
          this.showToast('error', this.errorMessage);
        }
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = err.message || 'Operation failed';
        this.showToast('error', this.errorMessage);
      }
    });
  }

  onCancel(): void { this.router.navigate(['/members']); }

  private showToast(type: ToastMessage['type'], message: string): void {
    this.toastMessages = ToastComponent.create(this.toastMessages, type, message);
  }
  onDismissToast(id: string): void {
    this.toastMessages = this.toastMessages.filter((t) => t.id !== id);
  }
}
