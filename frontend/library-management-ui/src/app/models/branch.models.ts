export interface BranchDto {
  id: string;
  name: string;
  address: string;
  phone: string;
  isActive: boolean;
}

export interface CreateBranchRequest {
  name: string;
  address: string;
  phone: string;
}

export interface UpdateBranchRequest {
  name: string;
  address: string;
  phone: string;
  isActive: boolean;
}
