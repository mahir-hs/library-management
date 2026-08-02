export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  fullName: string;
  phoneNumber: string;
  role: string;
  branchId: string;
}

export interface AuthResponse {
  userId: string;
  username: string;
  email: string;
  fullName: string;
  role: string;
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface UserDto {
  id: string;
  username: string;
  email: string;
  fullName: string;
  phoneNumber: string;
  role: string;
  isActive: boolean;
  lastLoginAt: string;
}
