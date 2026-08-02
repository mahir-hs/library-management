export interface BranchDto {
  id: string;
  name: string;
  code: string;
  address: string;
  phone: string | null;
  email: string | null;
  isActive: boolean;
  bookCopyCount: number;
  staffCount: number;
  createdAt: string;
}

export interface CreateBranchRequest {
  name: string;
  code: string;
  address: string;
  phone?: string;
  email?: string;
}

export interface UpdateBranchRequest {
  name: string;
  code: string;
  address: string;
  phone?: string;
  email?: string;
  isActive: boolean;
}

export interface BranchSearchResponse {
  id: string;
  name: string;
  code: string;
  address: string;
  phone: string | null;
  email: string | null;
  isActive: boolean;
  bookCopyCount: number;
}
