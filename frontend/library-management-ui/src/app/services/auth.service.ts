import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, RegisterRequest, AuthResponse, UserDto } from '../models/auth.models';
import { Result } from '../models/result.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<UserDto | null>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<Result<AuthResponse>> {
    return this.http.post<Result<AuthResponse>>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.storeAuthData(response.data);
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<Result<AuthResponse>> {
    return this.http.post<Result<AuthResponse>>(`${environment.apiUrl}/auth/register`, request).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.storeAuthData(response.data);
        }
      })
    );
  }

  logout(): void {
    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('refreshToken');
    sessionStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    return !!sessionStorage.getItem('authToken');
  }

  getRole(): string {
    const user = this.getUserFromStorage();
    return user?.role || '';
  }

  getCurrentUser(): UserDto | null {
    return this.getUserFromStorage();
  }

  getToken(): string | null {
    return sessionStorage.getItem('authToken');
  }

  private storeAuthData(response: AuthResponse): void {
    sessionStorage.setItem('authToken', response.token);
    sessionStorage.setItem('refreshToken', response.refreshToken);
    const user: UserDto = {
      id: response.userId,
      username: response.email,
      email: response.email,
      fullName: response.fullName,
      phoneNumber: '',
      role: response.role,
      isActive: true,
      lastLoginAt: new Date().toISOString(),
      branchId: ''
    };
    sessionStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private getUserFromStorage(): UserDto | null {
    const userJson = sessionStorage.getItem('currentUser');
    if (userJson) {
      try {
        return JSON.parse(userJson);
      } catch {
        return null;
      }
    }
    return null;
  }
}
