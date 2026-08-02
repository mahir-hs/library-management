import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { BorrowHistoryDto, OverdueBookDto, MemberActivityDto, ReportSummaryDto } from '../models/report.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  constructor(private api: ApiService) {}

  getSummary(): Observable<Result<ReportSummaryDto>> {
    return this.api.get<Result<ReportSummaryDto>>('/reports/summary');
  }

  getBorrowHistory(pageNumber: number = 1, pageSize: number = 10, startDate?: string, endDate?: string): Observable<Result<PaginatedResult<BorrowHistoryDto>>> {
    const params = new URLSearchParams();
    params.set('pageNumber', String(pageNumber));
    params.set('pageSize', String(pageSize));
    if (startDate) params.set('fromDate', startDate);
    if (endDate) params.set('toDate', endDate);
    return this.api.get<Result<PaginatedResult<BorrowHistoryDto>>>(`/reports/borrow-history?${params.toString()}`);
  }

  getOverdue(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<OverdueBookDto>>> {
    return this.api.get<Result<PaginatedResult<OverdueBookDto>>>(`/reports/overdue?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  getMemberActivity(memberId: string): Observable<Result<MemberActivityDto[]>> {
    return this.api.get<Result<MemberActivityDto[]>>(`/reports/member-activity/${memberId}`);
  }
}
