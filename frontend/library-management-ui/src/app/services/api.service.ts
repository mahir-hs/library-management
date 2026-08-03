import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  // Endpoints whose raw responses should NOT be wrapped in Result
  private readonly RAW_RESPONSE_URLS = [
    '/auth/login',
    '/auth/refresh',
    '/auth/register',
  ];

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = this.getToken();
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    return headers;
  }

  private getToken(): string | null {
    return sessionStorage.getItem('authToken');
  }

  private shouldWrapResponse(url: string): boolean {
    return !this.RAW_RESPONSE_URLS.some(rawUrl => url.includes(rawUrl));
  }

  get<T>(url: string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}${url}`, { headers: this.getHeaders() })
      .pipe(
        map((response: any) => {
          if (this.shouldWrapResponse(url) && response && typeof response === 'object' && !Array.isArray(response) && !response.success) {
            return { success: true, data: response, errors: null } as T;
          }
          return response;
        }),
        catchError(this.handleError)
      );
  }

  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${url}`, body, { headers: this.getHeaders() })
      .pipe(
        map((response: any) => {
          if (this.shouldWrapResponse(url) && response && typeof response === 'object' && !Array.isArray(response) && !response.success) {
            return { success: true, data: response, errors: null } as T;
          }
          return response;
        }),
        catchError(this.handleError)
      );
  }

  put<T>(url: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}${url}`, body, { headers: this.getHeaders() })
      .pipe(
        map((response: any) => {
          if (this.shouldWrapResponse(url) && response && typeof response === 'object' && !Array.isArray(response) && !response.success) {
            return { success: true, data: response, errors: null } as T;
          }
          return response;
        }),
        catchError(this.handleError)
      );
  }

  patch<T>(url: string, body?: any): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl}${url}`, body, { headers: this.getHeaders() })
      .pipe(
        map((response: any) => {
          if (this.shouldWrapResponse(url) && response && typeof response === 'object' && !Array.isArray(response) && !response.success) {
            return { success: true, data: response, errors: null } as T;
          }
          return response;
        }),
        catchError(this.handleError)
      );
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${url}`, { headers: this.getHeaders() })
      .pipe(
        map((response: any) => {
          if (this.shouldWrapResponse(url) && response && typeof response === 'object' && !Array.isArray(response) && !response.success) {
            return { success: true, data: response, errors: null } as T;
          }
          return response;
        }),
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Client error: ${error.error.message}`;
    } else {
      errorMessage = error.error?.message || error.error?.title || `Server error: ${error.status}`;
      // Handle ASP.NET Core ProblemDetails format
      if (error.error?.errors) {
        const errors = error.error.errors;
        const errorMessages = Object.values(errors).flat();
        errorMessage = errorMessages.join('; ');
      } else if (typeof error.error === 'string') {
        errorMessage = error.error;
      }
    }
    return throwError(() => new Error(errorMessage));
  }
}
