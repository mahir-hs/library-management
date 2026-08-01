import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { BookDto } from '../models/book.models';
import { CreateBookRequest, UpdateBookRequest } from '../models/book.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  constructor(private api: ApiService) {}

  getAll(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BookDto>>> {
    return this.api.get<PaginatedResult<BookDto>>(`/books?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  search(query: string, pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BookDto>>> {
    const params = new URLSearchParams();
    if (query) params.set('query', query);
    params.set('pageNumber', String(pageNumber));
    params.set('pageSize', String(pageSize));
    return this.api.get<PaginatedResult<BookDto>>(`/books/search?${params.toString()}`);
  }

  getById(id: string): Observable<Result<BookDto>> {
    return this.api.get<BookDto>(`/books/${id}`);
  }

  create(request: CreateBookRequest): Observable<Result<BookDto>> {
    return this.api.post<BookDto>('/books', request);
  }

  update(id: string, request: UpdateBookRequest): Observable<Result<BookDto>> {
    return this.api.put<BookDto>(`/books/${id}`, request);
  }

  delete(id: string): Observable<Result<void>> {
    return this.api.delete<void>(`/books/${id}`);
  }
}
