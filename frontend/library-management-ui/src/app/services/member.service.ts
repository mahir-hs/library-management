import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { MemberDto } from '../models/member.models';
import {
  CreateMemberRequest,
  UpdateMemberRequest,
} from '../models/member.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  constructor(private api: ApiService) {}

  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
  ): Observable<Result<PaginatedResult<MemberDto>>> {
    return this.api.get<PaginatedResult<MemberDto>>(
      `/members?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }

  search(
    query: string,
    pageNumber: number = 1,
    pageSize: number = 10,
  ): Observable<Result<PaginatedResult<MemberDto>>> {
    const params = new URLSearchParams();
    if (query) params.set('query', query);
    params.set('pageNumber', String(pageNumber));
    params.set('pageSize', String(pageSize));
    return this.api.get<PaginatedResult<MemberDto>>(
      `/members/search?${params.toString()}`,
    );
  }

  getById(id: string): Observable<Result<MemberDto>> {
    return this.api.get<MemberDto>(`/members/${id}`);
  }

  create(request: CreateMemberRequest): Observable<Result<MemberDto>> {
    return this.api.post<MemberDto>('/members', request);
  }

  update(
    id: string,
    request: UpdateMemberRequest,
  ): Observable<Result<MemberDto>> {
    return this.api.put<MemberDto>(`/members/${id}`, request);
  }

  delete(id: string): Observable<Result<void>> {
    return this.api.delete<void>(`/members/${id}`);
  }
}
