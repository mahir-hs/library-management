export interface MemberDto {
  id: string;
  userId: string;
  membershipNumber: string;
  fullName: string;
  email: string;
  phoneNumber: string | null;
  address: string;
  joinedDate: string;
  activeBorrows: number;
  totalBorrows: number;
  overdueBorrows: number;
}

export interface MemberDetailDto {
  id: string;
  userId: string;
  membershipNumber: string;
  fullName: string;
  email: string;
  phoneNumber: string | null;
  address: string;
  joinedDate: string;
  activeBorrows: number;
  totalBorrows: number;
  overdueBorrows: number;
  pendingReservations: number;
}

export interface CreateMemberRequest {
  userId: string;
  membershipNumber: string;
  address: string;
  phoneNumber?: string;
}

export interface UpdateMemberRequest {
  membershipNumber?: string;
  address?: string;
  phoneNumber?: string;
}
