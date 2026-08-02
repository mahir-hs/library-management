export interface BorrowDto {
  id: string;
  memberId: string;
  memberName: string;
  bookCopyId: string;
  bookTitle: string;
  bookIsbn: string;
  borrowedAt: string;
  dueDate: string;
  returnedAt: string | null;
  status: string;
  fineAmount: number;
  daysOverdue: number;
}

export interface CreateBorrowRequest {
  memberId: string;
  bookCopyId: string;
}

export interface ReturnBorrowRequest {
  fineAmount?: number;
}

export interface MyBorrowsResponse {
  id: string;
  bookTitle: string;
  bookIsbn: string;
  authorName: string;
  borrowedAt: string;
  dueDate: string;
  returnedAt: string | null;
  status: string;
  fineAmount: number;
}

export interface BorrowListResponse {
  id: string;
  memberName: string;
  bookTitle: string;
  bookIsbn: string;
  borrowedAt: string;
  dueDate: string;
  status: string;
  daysOverdue: number;
}
