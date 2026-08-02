import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { ReservationDto, MyReservationsResponse, ReservationQueueDto } from '../models/reservation.models';
import { CreateReservationRequest, CancelReservationRequest } from '../models/reservation.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root',
})
export class ReservationService {
  constructor(private api: ApiService) {}

  getAll(pageNumber: number = 1, pageSize: number = 10): Observable<Result<PaginatedResult<MyReservationsResponse>>> {
    return this.api.get<Result<PaginatedResult<MyReservationsResponse>>>(
      `/reservations/my?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }

  create(request: CreateReservationRequest): Observable<Result<ReservationDto>> {
    return this.api.post<Result<ReservationDto>>('/reservations', request);
  }

  cancel(id: string, reason: string): Observable<Result<ReservationDto>> {
    return this.api.patch<Result<ReservationDto>>(`/reservations/${id}/cancel`, { reason });
  }

  fulfill(id: string): Observable<Result<ReservationDto>> {
    return this.api.patch<Result<ReservationDto>>(`/reservations/${id}/fulfill`);
  }

  getQueue(bookId: string): Observable<ReservationQueueDto[]> {
    return this.api.get<ReservationQueueDto[]>(`/reservations/queue/${bookId}`);
  }
}
