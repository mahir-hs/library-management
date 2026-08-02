import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { UserDto } from '../models/auth.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private api: ApiService) {}

  getAll(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<UserDto>>> {
    return this.api.get<Result<PaginatedResult<UserDto>>>(`/users?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  getById(id: string): Observable<Result<UserDto>> {
    return this.api.get<Result<UserDto>>(`/users/${id}`);
  }

  delete(id: string): Observable<Result<void>> {
    return this.api.delete<Result<void>>(`/users/${id}`);
  }
}
