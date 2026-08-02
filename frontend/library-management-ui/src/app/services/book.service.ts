import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { BookDto, BookSearchResponse } from '../models/book.models';
import { CreateBookRequest, UpdateBookRequest } from '../models/book.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  constructor(private api: ApiService) {}

  getAll(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BookDto>>> {
    return this.api.get<Result<PaginatedResult<BookDto>>>(`/books?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  search(title?: string, author?: string, isbn?: string, categoryId?: string, pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BookSearchResponse>>> {
    const params = new URLSearchParams();
    if (title) params.set('title', title);
    if (author) params.set('author', author);
    if (isbn) params.set('isbn', isbn);
    if (categoryId) params.set('categoryId', categoryId);
    params.set('pageNumber', String(pageNumber));
    params.set('pageSize', String(pageSize));
    return this.api.get<Result<PaginatedResult<BookSearchResponse>>>(`/books/search?${params.toString()}`);
  }

  getById(id: string): Observable<Result<BookDto>> {
    return this.api.get<Result<BookDto>>(`/books/${id}`);
  }

  create(request: CreateBookRequest): Observable<Result<BookDto>> {
    return this.api.post<Result<BookDto>>('/books', request);
  }

  update(id: string, request: UpdateBookRequest): Observable<Result<BookDto>> {
    return this.api.put<Result<BookDto>>(`/books/${id}`, request);
  }

  delete(id: string): Observable<Result<void>> {
    return this.api.delete<Result<void>>(`/books/${id}`);
  }

  addCopies(id: string, request: { quantity: number; branchId: string; shelfLocation: string }): Observable<Result<BookDto>> {
    return this.api.post<Result<BookDto>>(`/books/${id}/copies`, request);
  }
}
