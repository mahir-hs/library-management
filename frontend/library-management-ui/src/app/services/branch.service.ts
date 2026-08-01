import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { BranchDto } from '../models/branch.models';
import { CreateBranchRequest, UpdateBranchRequest } from '../models/branch.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root'
})
export class BranchService {
  constructor(private api: ApiService) {}

  getAll(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BranchDto>>> {
    return this.api.get<PaginatedResult<BranchDto>>(`/branches?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  search(name?: string, code?: string, isActive?: boolean, pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<BranchDto>>> {
    const params = new URLSearchParams();
    if (name) params.set('name', name);
    if (code) params.set('code', code);
    if (isActive !== undefined) params.set('isActive', String(isActive));
    params.set('pageNumber', String(pageNumber));
    params.set('pageSize', String(pageSize));
    return this.api.get<PaginatedResult<BranchDto>>(`/branches/search?${params.toString()}`);
  }

  getById(id: string): Observable<Result<BranchDto>> {
    return this.api.get<BranchDto>(`/branches/${id}`);
  }

  create(request: CreateBranchRequest): Observable<Result<BranchDto>> {
    return this.api.post<BranchDto>('/branches', request);
  }

  update(id: string, request: UpdateBranchRequest): Observable<Result<BranchDto>> {
    return this.api.put<BranchDto>(`/branches/${id}`, request);
  }

  delete(id: string): Observable<Result<void>> {
    return this.api.delete<void>(`/branches/${id}`);
  }
}
