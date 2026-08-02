import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { BranchService } from '../../services/branch.service';
import { BranchDto } from '../../models/branch.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  form = {
    username: '',
    email: '',
    password: '',
    fullName: '',
    phoneNumber: '',
    role: 'Member',
    branchId: ''
  };
  branches: BranchDto[] = [];
  errorMessage = '';
  successMessage = '';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private branchService: BranchService
  ) {}

  ngOnInit(): void {
    this.branchService.getAll(1, 100).subscribe({
      next: (response) => {
        this.branches = response.items;
      },
      error: () => {
        this.branches = [];
      }
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
      }
    });
  }
}
