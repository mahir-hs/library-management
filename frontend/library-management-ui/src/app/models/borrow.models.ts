export interface BorrowDto {
  id: string;
  bookCopyId: string;
  bookTitle: string;
  memberName: string;
  borrowedDate: string;
  dueDate: string;
  returnedDate: string | null;
  status: string;
  fineAmount: number;
}

export interface CreateBorrowRequest {
  bookCopyId: string;
  memberId: string;
}

export interface ReturnBorrowRequest {
  borrowId: string;
  fineAmount?: number;
}
