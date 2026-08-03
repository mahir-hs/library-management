import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';
import { LoginComponent } from './components/auth/login.component';
import { RegisterComponent } from './components/auth/register.component';
import { AppLayoutComponent } from './components/layout/app-layout.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ProfileComponent } from './components/profile/profile.component';
import { UsersListComponent } from './components/users/users-list/users-list.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: AppLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', component: DashboardComponent, canActivate: [authGuard] },
      { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
      { path: 'register', component: RegisterComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Librarian'] } },
      { path: 'branches', loadComponent: () => import('./components/branches/branch-list/branch-list.component').then(m => m.BranchListComponent), canActivate: [authGuard] },
      { path: 'branches/new', loadComponent: () => import('./components/branches/branch-form/branch-form.component').then(m => m.BranchFormComponent), canActivate: [authGuard] },
      { path: 'branches/:id', loadComponent: () => import('./components/branches/branch-detail/branch-detail.component').then(m => m.BranchDetailComponent), canActivate: [authGuard] },
      { path: 'branches/:id/edit', loadComponent: () => import('./components/branches/branch-form/branch-form.component').then(m => m.BranchFormComponent), canActivate: [authGuard] },
      { path: 'books', loadComponent: () => import('./components/books/book-list/book-list.component').then(m => m.BookListComponent), canActivate: [authGuard] },
      { path: 'books/new', loadComponent: () => import('./components/books/book-form/book-form.component').then(m => m.BookFormComponent), canActivate: [authGuard] },
      { path: 'books/:id', loadComponent: () => import('./components/books/book-detail/book-detail.component').then(m => m.BookDetailComponent), canActivate: [authGuard] },
      { path: 'books/:id/edit', loadComponent: () => import('./components/books/book-form/book-form.component').then(m => m.BookFormComponent), canActivate: [authGuard] },
      { path: 'members', loadComponent: () => import('./components/members/member-list/member-list.component').then(m => m.MemberListComponent), canActivate: [authGuard] },
      { path: 'members/new', loadComponent: () => import('./components/members/member-form/member-form.component').then(m => m.MemberFormComponent), canActivate: [authGuard] },
      { path: 'members/:id', loadComponent: () => import('./components/members/member-detail/member-detail.component').then(m => m.MemberDetailComponent), canActivate: [authGuard] },
      { path: 'members/:id/edit', loadComponent: () => import('./components/members/member-form/member-form.component').then(m => m.MemberFormComponent), canActivate: [authGuard] },
      { path: 'borrows', loadComponent: () => import('./components/borrows/borrow-list/borrow-list.component').then(m => m.BorrowListComponent), canActivate: [authGuard] },
      { path: 'borrows/new', loadComponent: () => import('./components/borrows/borrow-form/borrow-form.component').then(m => m.BorrowFormComponent), canActivate: [authGuard] },
      { path: 'borrows/:id', loadComponent: () => import('./components/borrows/borrow-detail/borrow-detail.component').then(m => m.BorrowDetailComponent), canActivate: [authGuard] },
      { path: 'reservations', loadComponent: () => import('./components/reservations/reservation-list/reservation-list.component').then(m => m.ReservationListComponent), canActivate: [authGuard] },
      { path: 'reports', loadComponent: () => import('./components/reports/reports-page/reports-page.component').then(m => m.ReportsPageComponent), canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Librarian'] } },
      { path: 'users', loadComponent: () => import('./components/users/users-list/users-list.component').then(m => m.UsersListComponent), canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Librarian'] } },
      { path: '**', redirectTo: '' }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
