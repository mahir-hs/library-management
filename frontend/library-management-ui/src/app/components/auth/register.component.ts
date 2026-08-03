import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { BranchService } from '../../services/branch.service';
import { BranchDto } from '../../models/branch.models';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  form = {
    username: '',
    email: '',
    password: '',
    fullName: '',
    phoneNumber: '',
    role: 'Member',
    branchId: '',
  };
  branches: BranchDto[] = [];
  errorMessage = '';
  successMessage = '';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private branchService: BranchService,
  ) {}

  ngOnInit(): void {
    const userRole = this.authService.getRole();

    // Only Admin and Librarian can access registration
    if (!userRole || !['Admin', 'Librarian'].includes(userRole)) {
      this.router.navigate(['/']);
      return;
    }

    // Set default role based on current user's role
    if (userRole === 'Librarian') {
      this.form.role = 'Member';
    } else {
      this.form.role = 'Admin';
    }

    this.branchService.getAll(1, 100).subscribe({
      next: (response) => {
        // Unwrap response.data to reach .items
        if (response.success && response.data) {
          this.branches = response.data.items;
        } else {
          this.branches = [];
        }
      },
      error: () => {
        this.branches = [];
      },
    });
  }

  get availableRoles(): string[] {
    const userRole = this.authService.getRole();
    if (userRole === 'Admin') {
      return ['Admin', 'Librarian', 'Member'];
    }
    // Librarian can only register Members
    return ['Member'];
  }

  onRegister(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.loading = true;

    this.authService.register(this.form).subscribe({
      next: () => {
        this.loading = false;
        this.successMessage = 'Registration successful! Please log in.';
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Registration failed';
      },
    });
  }
}
