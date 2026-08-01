export interface BorrowHistoryDto {
  id: string;
  bookTitle: string;
  memberName: string;
  borrowedDate: string;
  dueDate: string;
  returnedDate: string | null;
  status: string;
}

export interface OverdueBookDto {
  id: string;
  bookTitle: string;
  memberName: string;
  daysOverdue: number;
  fineAmount: number;
}

export interface MemberActivityDto {
  memberId: string;
  memberName: string;
  totalBorrows: number;
  activeBorrows: number;
  overdueBorrows: number;
  totalFines: number;
}

export interface ReportSummaryDto {
  totalBooks: number;
  activeBorrows: number;
  pendingReservations: number;
  overdueCount: number;
}
