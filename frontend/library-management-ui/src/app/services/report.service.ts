import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { BorrowHistoryDto, OverdueBookDto, MemberActivityDto, ReportSummaryDto } from '../models/report.models';
import { Result } from '../models/result.models';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  constructor(private api: ApiService) {}

  getBorrowHistory(startDate?: string, endDate?: string): Observable<Result<BorrowHistoryDto[]>> {
    let url = '/reports/borrow-history';
    const params: string[] = [];
    if (startDate) params.push(`startDate=${startDate}`);
    if (endDate) params.push(`endDate=${endDate}`);
    if (params.length > 0) url += `?${params.join('&')}`;
    return this.api.get<BorrowHistoryDto[]>(url);
  }

  getOverdue(): Observable<Result<OverdueBookDto[]>> {
    return this.api.get<OverdueBookDto[]>('/reports/overdue');
  }

  getMemberActivity(memberId: string): Observable<Result<MemberActivityDto>> {
    return this.api.get<MemberActivityDto>(`/reports/member-activity/${memberId}`);
  }

  getSummary(): Observable<Result<ReportSummaryDto>> {
    return this.api.get<ReportSummaryDto>('/reports/summary');
  }
}
