import { api } from "./client";
import { tokenStore } from "./tokens";
import type { AuthResponse, LoginRequest, RegisterRequest, ResetPasswordRequest, User } from "./types";

async function startSession(promise: Promise<AuthResponse>): Promise<User> {
  const auth = await promise;
  tokenStore.save(auth);
  return auth.user;
}

export const authApi = {
  register: (request: RegisterRequest) =>
    startSession(api.post<AuthResponse>("/api/auth/register", request, { auth: false })),

  login: (request: LoginRequest) =>
    startSession(api.post<AuthResponse>("/api/auth/login", request, { auth: false })),

  async logout(): Promise<void> {
    const refreshToken = tokenStore.getRefreshToken();
    tokenStore.clear();
    if (refreshToken) {
      await api.post("/api/auth/logout", { refreshToken }, { auth: false }).catch(() => undefined);
    }
  },

  /** Prisijungęs vartotojas; jei prieigos žetono nėra, bandoma atkurti sesiją iš atnaujinimo žetono. */
  me: () => api.get<User>("/api/auth/me"),

  forgotPassword: (email: string) => api.post("/api/auth/forgot-password", { email }, { auth: false }),

  resetPassword: (request: ResetPasswordRequest) =>
    api.post("/api/auth/reset-password", request, { auth: false }),
};
