import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, RegisterRequest, AuthResponse, UserDto } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<UserDto | null>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap(response => {
        this.storeAuthData(response);
      })
    );
  }

  register(request: RegisterRequest): Observable<UserDto> {
    return this.http.post<UserDto>(`${environment.apiUrl}/auth/register`, request).pipe(
      tap(response => {
        // Registration doesn't auto-login; user must log in separately
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

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = sessionStorage.getItem('refreshToken');
    const accessToken = sessionStorage.getItem('authToken');
    if (!refreshToken || !accessToken) {
      throw new Error('No refresh token available');
    }
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/refresh`, {
      accessToken,
      refreshToken
    }).pipe(
      tap(response => {
        this.storeAuthData(response);
      })
    );
  }

  private storeAuthData(response: AuthResponse): void {
    sessionStorage.setItem('authToken', response.accessToken);
    sessionStorage.setItem('refreshToken', response.refreshToken);
    const user: UserDto = {
      id: response.userId,
      username: response.username,
      email: response.email,
      fullName: response.fullName,
      phoneNumber: '',
      role: response.role,
      isActive: true,
      lastLoginAt: new Date().toISOString()
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
