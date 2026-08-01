export interface ReservationDto {
  id: string;
  bookTitle: string;
  memberName: string;
  status: string;
  positionInQueue: number;
  createdAt: string;
}

export interface CreateReservationRequest {
  bookId: string;
}

export interface CancelReservationRequest {
  reservationId: string;
}
