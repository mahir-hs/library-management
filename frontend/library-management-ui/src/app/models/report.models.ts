export interface BorrowHistoryDto {
  id: string;
  memberName: string;
  bookTitle: string;
  isbn: string;
  authorName: string;
  borrowedAt: string;
  dueDate: string;
  returnedAt: string | null;
  daysKept: number;
  fineAmount: number;
}

export interface OverdueBookDto {
  id: string;
  borrowId: string;
  memberName: string;
  bookTitle: string;
  isbn: string;
  dueDate: string;
  daysOverdue: number;
  estimatedFine: number;
  fineAmount: number;
}

export interface MemberActivityDto {
  memberId: string;
  memberName: string;
  membershipNumber: string;
  totalBorrows: number;
  activeBorrows: number;
  overdueBorrows: number;
  pendingReservations: number;
  totalFines: number;
  joinedDate: string;
}

export interface ReportSummaryDto {
  totalBooks: number;
  totalAvailableCopies: number;
  totalBorrowedCopies: number;
  totalMembers: number;
  activeBorrows: number;
  overdueBorrows: number;
  pendingReservations: number;
  totalOutstandingFines: number;
  overdueCount: number;
  memberSummary: MemberActivityDto[];
}
