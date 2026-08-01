import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Result } from '../models/result.models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

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

  get<T>(url: string): Observable<Result<T>> {
    return this.http.get<Result<T>>(`${this.baseUrl}${url}`, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  post<T>(url: string, body: any): Observable<Result<T>> {
    return this.http.post<Result<T>>(`${this.baseUrl}${url}`, body, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  put<T>(url: string, body: any): Observable<Result<T>> {
    return this.http.put<Result<T>>(`${this.baseUrl}${url}`, body, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  patch<T>(url: string, body?: any): Observable<Result<T>> {
    return this.http.patch<Result<T>>(`${this.baseUrl}${url}`, body, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  delete<T>(url: string): Observable<Result<T>> {
    return this.http.delete<Result<T>>(`${this.baseUrl}${url}`, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Client error: ${error.error.message}`;
    } else {
      errorMessage = `Server error: ${error.status} - ${error.message}`;
      if (error.error && error.error.errors) {
        errorMessage = error.error.errors.join('; ');
      }
    }
    return throwError(() => new Error(errorMessage));
  }
}
