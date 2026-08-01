export interface MemberDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  membershipDate: string;
  branchId: string;
  branchName: string;
}

export interface CreateMemberRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  membershipDate: string;
  branchId: string;
}

export interface UpdateMemberRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  membershipDate: string;
  branchId: string;
}
