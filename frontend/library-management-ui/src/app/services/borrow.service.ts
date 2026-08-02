import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { BorrowDto, MyBorrowsResponse, BorrowListResponse } from '../models/borrow.models';
import { CreateBorrowRequest, ReturnBorrowRequest } from '../models/borrow.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root',
})
export class BorrowService {
  constructor(private api: ApiService) {}

  create(request: CreateBorrowRequest): Observable<Result<BorrowDto>> {
    return this.api.post<Result<BorrowDto>>('/borrows', request);
  }

  returnBorrow(id: string, fineAmount?: number): Observable<Result<BorrowDto>> {
    return this.api.put<Result<BorrowDto>>(`/borrows/${id}/return`, { fineAmount });
  }

  getById(id: string): Observable<Result<BorrowDto>> {
    return this.api.get<Result<BorrowDto>>(`/borrows/${id}`);
  }

  getMine(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<MyBorrowsResponse>>> {
    return this.api.get<Result<PaginatedResult<MyBorrowsResponse>>>(
      `/borrows/my?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }

  getByMember(memberId: string, pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BorrowDto>>> {
    return this.api.get<Result<PaginatedResult<BorrowDto>>>(
      `/borrows/member/${memberId}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }

  getOverdue(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BorrowListResponse>>> {
    return this.api.get<Result<PaginatedResult<BorrowListResponse>>>(
      `/borrows/overdue?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }
}
