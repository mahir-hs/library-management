import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { BranchDto, BranchSearchResponse } from '../models/branch.models';
import { CreateBranchRequest, UpdateBranchRequest } from '../models/branch.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root'
})
export class BranchService {
  constructor(private api: ApiService) {}

  getAll(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BranchDto>>> {
    return this.api.get<Result<PaginatedResult<BranchDto>>>(`/branches/all?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  search(name?: string, code?: string, isActive?: boolean, pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BranchSearchResponse>>> {
    const params = new URLSearchParams();
    if (name) params.set('name', name);
    if (code) params.set('code', code);
    if (isActive !== undefined) params.set('isActive', String(isActive));
    params.set('pageNumber', String(pageNumber));
    params.set('pageSize', String(pageSize));
    return this.api.get<Result<PaginatedResult<BranchSearchResponse>>>(`/branches/search?${params.toString()}`);
  }

  getById(id: string): Observable<Result<BranchDto>> {
    return this.api.get<Result<BranchDto>>(`/branches/${id}`);
  }

  getByCode(code: string): Observable<Result<BranchDto>> {
    return this.api.get<Result<BranchDto>>(`/branches/code/${code}`);
  }

  create(request: CreateBranchRequest): Observable<Result<BranchDto>> {
    return this.api.post<Result<BranchDto>>('/branches', request);
  }

  update(id: string, request: UpdateBranchRequest): Observable<Result<BranchDto>> {
    return this.api.put<Result<BranchDto>>(`/branches/${id}`, request);
  }

  delete(id: string): Observable<Result<void>> {
    return this.api.delete<Result<void>>(`/branches/${id}`);
  }
}
