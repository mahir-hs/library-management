import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { BorrowDto } from '../models/borrow.models';
import {
  CreateBorrowRequest,
  ReturnBorrowRequest,
} from '../models/borrow.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root',
})
export class BorrowService {
  constructor(private api: ApiService) {}

  create(request: CreateBorrowRequest): Observable<Result<BorrowDto>> {
    return this.api.post<BorrowDto>('/borrows', request);
  }

  returnBorrow(request: ReturnBorrowRequest): Observable<Result<BorrowDto>> {
    return this.api.put<BorrowDto>(`/borrows/${request.borrowId}/return`, {});
  }

  getById(id: string): Observable<Result<BorrowDto>> {
    return this.api.get<BorrowDto>(`/borrows/${id}`);
  }

  getMine(
    pageNumber: number = 1,
    pageSize: number = 10,
  ): Observable<Result<PaginatedResult<BorrowDto>>> {
    return this.api.get<PaginatedResult<BorrowDto>>(
      `/borrows/mine?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }

  getByMember(
    memberId: string,
    pageNumber: number = 1,
    pageSize: number = 10,
  ): Observable<Result<PaginatedResult<BorrowDto>>> {
    return this.api.get<PaginatedResult<BorrowDto>>(
      `/borrows/member/${memberId}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }

  getOverdue(
    pageNumber: number = 1,
    pageSize: number = 10,
  ): Observable<Result<PaginatedResult<BorrowDto>>> {
    return this.api.get<PaginatedResult<BorrowDto>>(
      `/borrows/overdue?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }
}
