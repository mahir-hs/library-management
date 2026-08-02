export interface ReservationDto {
  id: string;
  memberId: string;
  memberName: string;
  bookId: string;
  bookTitle: string;
  isbn: string;
  status: string;
  reservedAt: string;
  expiresAt: string | null;
}

export interface CreateReservationRequest {
  memberId: string;
  bookId: string;
}

export interface CancelReservationRequest {
  reason: string;
}

export interface MyReservationsResponse {
  id: string;
  bookTitle: string;
  isbn: string;
  authorName: string;
  categoryName: string;
  status: string;
  reservedAt: string;
  expiresAt: string | null;
  memberName: string;
  positionInQueue: number;
}

export interface ReservationQueueDto {
  id: string;
  memberName: string;
  bookTitle: string;
  isbn: string;
  positionInQueue: number;
  reservedAt: string;
  expiresAt: string | null;
  status: string;
}
