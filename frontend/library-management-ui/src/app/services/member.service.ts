import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { MemberDto, MemberDetailDto } from '../models/member.models';
import { CreateMemberRequest, UpdateMemberRequest } from '../models/member.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  constructor(private api: ApiService) {}

  getAll(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<MemberDto>>> {
    return this.api.get<Result<PaginatedResult<MemberDto>>>(
      `/members/all?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }

  getById(id: string): Observable<Result<MemberDetailDto>> {
    return this.api.get<Result<MemberDetailDto>>(`/members/${id}`);
  }

  getByUserId(userId: string): Observable<Result<MemberDetailDto>> {
    return this.api.get<Result<MemberDetailDto>>(`/members/user/${userId}`);
  }

  getMyProfile(): Observable<Result<MemberDetailDto>> {
    return this.api.get<Result<MemberDetailDto>>('/members/me');
  }

  create(request: CreateMemberRequest): Observable<Result<MemberDetailDto>> {
    return this.api.post<Result<MemberDetailDto>>('/members', request);
  }

  update(id: string, request: UpdateMemberRequest): Observable<Result<MemberDto>> {
    return this.api.put<Result<MemberDto>>(`/members/${id}`, request);
  }

  delete(id: string): Observable<Result<void>> {
    return this.api.delete<Result<void>>(`/members/${id}`);
  }
}
