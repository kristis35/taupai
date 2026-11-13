/** Backend'o DTO atitikmenys (backend/src/SaskaituAgentas.Api/Dtos). */

export type HouseholdRole = "Owner" | "Member";

export interface User {
  id: string;
  email: string;
  fullName: string;
  householdId: string;
  role: HouseholdRole;
  createdAt: string;
}

export interface AuthResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
  user: User;
}

export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
  householdName?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

export interface Household {
  id: string;
  name: string;
  createdAt: string;
  updatedAt: string;
}

export type ContractCategory = "Electricity" | "Mobile" | "Internet" | "Tv" | "Subscription";

export interface Contract {
  id: string;
  householdId: string;
  category: ContractCategory;
  providerName: string;
  planName: string | null;
  monthlyCost: number;
  startDate: string | null;
  endDate: string | null;
  createdAt: string;
  updatedAt: string;
}

/** RFC 9457 Problem Details – visų API klaidų formatas. */
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  errors?: Record<string, string[]>;
  [key: string]: unknown;
}
