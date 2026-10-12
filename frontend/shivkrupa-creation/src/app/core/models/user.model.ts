export interface User {
  id: string;
  fullName: string;
  email: string;
  phoneNumber?: string | null;
  businessName?: string | null;
  city?: string | null;
  state?: string | null;
  isActive: boolean;
  roles: string[];
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
  phoneNumber?: string;
  businessName?: string;
  gstNumber?: string;
  city?: string;
  state?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RefreshTokenRequest {
  accessToken: string;
  refreshToken: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
  user: User;
}

export interface CustomerProfile extends User {
  gstNumber?: string | null;
  address?: string | null;
  pinCode?: string | null;
  createdAt: string;
}

export interface UpdateProfileRequest {
  fullName: string;
  phoneNumber?: string;
  businessName?: string;
  gstNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  pinCode?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}
