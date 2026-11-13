export { api, apiFetch, ApiError } from "./client";
export { authApi } from "./auth";
export { tokenStore } from "./tokens";
export type * from "./types";

import { api } from "./client";

export interface HealthReport {
  status: "Healthy" | "Degraded" | "Unhealthy";
  totalDurationMs: number;
  checks: Record<string, { status: string; durationMs: number; description: string | null; error: string | null }>;
}

export const healthApi = {
  ready: () => api.get<HealthReport>("/health/ready", { auth: false }),
};
