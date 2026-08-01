import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { ReservationDto } from '../models/reservation.models';
import {
  CreateReservationRequest,
  CancelReservationRequest,
} from '../models/reservation.models';
import { PaginatedResult, Result } from '../models/result.models';

@Injectable({
  providedIn: 'root',
})
export class ReservationService {
  constructor(private api: ApiService) {}

  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
  ): Observable<Result<PaginatedResult<ReservationDto>>> {
    return this.api.get<PaginatedResult<ReservationDto>>(
      `/reservations?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    );
  }

  create(
    request: CreateReservationRequest,
  ): Observable<Result<ReservationDto>> {
    return this.api.post<ReservationDto>('/reservations', request);
  }

  cancel(request: CancelReservationRequest): Observable<Result<void>> {
    return this.api.patch<void>(
      `/reservations/${request.reservationId}/cancel`,
    );
  }

  fulfill(id: string): Observable<Result<void>> {
    return this.api.patch<void>(`/reservations/${id}/fulfill`);
  }
}
