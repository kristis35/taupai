import { env } from "@/lib/env";
import { tokenStore } from "./tokens";
import type { AuthResponse, ProblemDetails } from "./types";

export class ApiError extends Error {
  constructor(
    readonly status: number,
    readonly problem: ProblemDetails | null,
  ) {
    super(problem?.detail ?? problem?.title ?? `API klaida (${status})`);
    this.name = "ApiError";
  }

  /** Validacijos klaidos pagal lauką (iš [ApiController] modelio validacijos). */
  get fieldErrors(): Record<string, string[]> {
    return this.problem?.errors ?? {};
  }
}

type Method = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export interface RequestOptions {
  method?: Method;
  body?: unknown;
  /** false – nesiųsti Authorization antraštės (pvz. login/register). Numatytai true. */
  auth?: boolean;
  signal?: AbortSignal;
  headers?: HeadersInit;
}

let refreshInFlight: Promise<boolean> | null = null;

/** Vienu metu vykdomas tik vienas atnaujinimas, kiti 401 atsakymai jo palaukia. */
function refreshSession(): Promise<boolean> {
  refreshInFlight ??= (async () => {
    const refreshToken = tokenStore.getRefreshToken();
    if (!refreshToken) return false;
    try {
      const res = await fetch(`${env.apiUrl}/api/auth/refresh`, {
        method: "POST",
        headers: { "Content-Type": "application/json", Accept: "application/json" },
        body: JSON.stringify({ refreshToken }),
      });
      if (!res.ok) {
        tokenStore.clear();
        return false;
      }
      tokenStore.save((await res.json()) as AuthResponse);
      return true;
    } catch {
      return false;
    } finally {
      refreshInFlight = null;
    }
  })();
  return refreshInFlight;
}

async function send(path: string, options: RequestOptions): Promise<Response> {
  const headers = new Headers(options.headers);
  headers.set("Accept", "application/json");
  if (options.body !== undefined) headers.set("Content-Type", "application/json");

  const token = options.auth === false ? null : tokenStore.getAccessToken();
  if (token) headers.set("Authorization", `Bearer ${token}`);

  return fetch(`${env.apiUrl}${path}`, {
    method: options.method ?? "GET",
    headers,
    body: options.body === undefined ? undefined : JSON.stringify(options.body),
    signal: options.signal,
  });
}

/**
 * Bendras API klientas: JSON, Bearer žetonas, automatinis sesijos atnaujinimas gavus 401,
 * klaidos kaip {@link ApiError} su Problem Details turiniu.
 */
export async function apiFetch<T = void>(path: string, options: RequestOptions = {}): Promise<T> {
  let res = await send(path, options);

  if (res.status === 401 && options.auth !== false && tokenStore.getRefreshToken()) {
    if (await refreshSession()) res = await send(path, options);
  }

  if (!res.ok) {
    let problem: ProblemDetails | null = null;
    try {
      problem = (await res.json()) as ProblemDetails;
    } catch {
      /* ne JSON atsakymas */
    }
    throw new ApiError(res.status, problem);
  }

  if (res.status === 204 || res.headers.get("Content-Length") === "0") return undefined as T;
  const text = await res.text();
  return (text ? JSON.parse(text) : undefined) as T;
}

export const api = {
  get: <T>(path: string, options?: Omit<RequestOptions, "method" | "body">) =>
    apiFetch<T>(path, { ...options, method: "GET" }),
  post: <T = void>(path: string, body?: unknown, options?: Omit<RequestOptions, "method" | "body">) =>
    apiFetch<T>(path, { ...options, method: "POST", body }),
  put: <T = void>(path: string, body?: unknown, options?: Omit<RequestOptions, "method" | "body">) =>
    apiFetch<T>(path, { ...options, method: "PUT", body }),
  delete: (path: string, options?: Omit<RequestOptions, "method" | "body">) =>
    apiFetch<void>(path, { ...options, method: "DELETE" }),
};
