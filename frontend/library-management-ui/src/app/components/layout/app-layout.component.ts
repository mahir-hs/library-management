import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.scss'
})
export class AppLayoutComponent {
  sidebarOpen = true;

  constructor(
    public authService: AuthService,
    private router: Router
  ) {}

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  get isAdmin(): boolean {
    return this.authService.getRole() === 'Admin';
  }

  get isAdminOrLibrarian(): boolean {
    return this.authService.getRole() === 'Admin' || this.authService.getRole() === 'Librarian';
  }

  get isMember(): boolean {
    return this.authService.getRole() === 'Member';
  }

  isMobile(): boolean {
    return window.innerWidth < 768;
  }
}
