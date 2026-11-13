# frontend

Next.js (App Router) + TypeScript + Tailwind CSS v4 + shadcn/ui.

```bash
npm install        # iš repozitorijos šaknies
npm run dev        # http://localhost:3000
```

shadcn/ui komponentai pridedami taip (paleisti `frontend/` kataloge):

```bash
npx shadcn@latest add card
```

Konfigūracija: `components.json`; komponentai – `src/components/ui/`; `cn()` – `src/lib/utils.ts`.

## Aplinkos kintamieji

```bash
cp .env.example .env.local
```

| Kintamasis | Paskirtis |
|------------|-----------|
| `NEXT_PUBLIC_API_URL` | Backend API adresas (numatytai `http://localhost:5109`) |

Backend'e šio frontend'o kilmė turi būti `Cors__AllowedOrigins` sąraše.

## API klientas

`src/lib/api/`:

- `client.ts` – `api.get/post/put/delete` ir `apiFetch`: JSON, `Authorization: Bearer`, automatinis sesijos
  atnaujinimas gavus 401, klaidos kaip `ApiError` (su Problem Details ir `fieldErrors`).
- `auth.ts` – `authApi.register/login/logout/me/forgotPassword/resetPassword`.
- `tokens.ts` – prieigos žetonas laikomas atmintyje, atnaujinimo žetonas – `localStorage`.
- `types.ts` – backend'o DTO tipai.

```ts
import { authApi, api, ApiError, type Contract } from "@/lib/api";

const user = await authApi.login({ email, password });
const contracts = await api.get<Contract[]>(`/api/households/${user.householdId}/contracts`);
```
