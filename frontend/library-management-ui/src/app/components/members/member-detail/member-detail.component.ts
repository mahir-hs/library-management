import { Component, OnInit } from '@angular/core';
import { MemberService } from '../../../services/member.service';
import { MemberDto } from '../../../models/member.models';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ToastComponent, ToastMessage } from '../../../shared/components/toast/toast.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, SpinnerComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.scss'
})
export class MemberDetailComponent implements OnInit {
  member: MemberDto | null = null;
  loading = false;
  errorMessage = '';
  toastMessages: ToastMessage[] = [];

  constructor(private memberService: MemberService, private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadMember(id);
  }

  loadMember(id: string): void {
    this.loading = true;
    this.memberService.getById(id).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.member = response.data;
        } else {
          this.errorMessage = response.errors?.join(' ') || 'Member not found';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Failed to load member';
      }
    });
  }

  onEdit(): void {
    if (this.member) {
      this.router.navigate(['/members', this.member.id, 'edit'], { state: { memberId: this.member.id } });
    }
  }
  onBack(): void { this.router.navigate(['/members']); }
  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }
}
