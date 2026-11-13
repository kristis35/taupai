import type { AuthResponse } from "./types";

/**
 * Žetonų saugykla naršyklėje. Prieigos žetonas laikomas tik atmintyje,
 * atnaujinimo žetonas – localStorage, kad sesija išliktų perkrovus puslapį.
 */
const REFRESH_KEY = "sa.refreshToken";

let accessToken: string | null = null;

const storage = () => (typeof window === "undefined" ? null : window.localStorage);

export const tokenStore = {
  getAccessToken: () => accessToken,
  getRefreshToken: () => {
    try {
      return storage()?.getItem(REFRESH_KEY) ?? null;
    } catch {
      return null;
    }
  },
  save(auth: Pick<AuthResponse, "accessToken" | "refreshToken">) {
    accessToken = auth.accessToken;
    try {
      storage()?.setItem(REFRESH_KEY, auth.refreshToken);
    } catch {
      /* privatus režimas ir pan. – sesija liks tik atmintyje */
    }
  },
  clear() {
    accessToken = null;
    try {
      storage()?.removeItem(REFRESH_KEY);
    } catch {
      /* ignoruojame */
    }
  },
};
