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
    role: 'Librarian',
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
