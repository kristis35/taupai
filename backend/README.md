# backend

Serverio dalis: ASP.NET Core Web API (.NET 10) + PostgreSQL per EF Core (Npgsql).

## Struktūra

```
backend/
├── SaskaituAgentas.slnx
├── dotnet-tools.json                 # lokalus dotnet-ef įrankis
└── src/SaskaituAgentas.Api/
    ├── Auth/                         # JWT, žetonai, prieigos prie namų ūkio filtras, el. paštas
    ├── Configuration/                # .env skaitytuvas, CORS, health checks, OpenAPI
    ├── Controllers/                  # HTTP sluoksnis (ploni kontroleriai)
    ├── Services/                     # verslo logika, ServiceResult
    ├── Repositories/                 # duomenų prieiga per EF Core
    ├── Data/
    │   ├── AppDbContext.cs
    │   ├── Configurations/           # IEntityTypeConfiguration<T>
    │   ├── Migrations/               # EF Core migracijos
    │   └── DesignTimeDbContextFactory.cs
    ├── Models/                       # domeno esybės (User, Household, Contract, žetonai)
    └── Dtos/                         # užklausų / atsakymų modeliai
```

Srautas: `Controller → Service → Repository → AppDbContext`. Priklausomybės registruojamos `Program.cs`.
Lentelių ir stulpelių pavadinimai DB – `snake_case` (EFCore.NamingConventions).

## Paleidimas lokaliai

Reikia [.NET 10 SDK](https://dotnet.microsoft.com/download) ir Docker.

```bash
# 1. PostgreSQL (iš repozitorijos šaknies)
docker compose -f infrastructure/docker/docker-compose.yml up -d

# 2. API (iš backend/)
cd backend
cp .env.example .env   # neprivaloma Development aplinkoje
dotnet tool restore
dotnet run --project src/SaskaituAgentas.Api
```

- API: <http://localhost:5109>
- Swagger UI: <http://localhost:5109/swagger> (OpenAPI schema – `/openapi/v1.json`). Mygtuku „Authorize“ įveskite
  `accessToken` iš `/api/auth/login`.
- Užklausų pavyzdžiai – `src/SaskaituAgentas.Api/SaskaituAgentas.Api.http`.

`Development` aplinkoje migracijos pritaikomos automatiškai paleidžiant (`Database:MigrateOnStartup`).

## Konfigūracija ir `.env`

Paleidžiant API ieško `.env` failo nuo darbinio katalogo aukštyn iki repozitorijos šaknies ir jo reikšmes įrašo į
aplinkos kintamuosius (jau nustatyti kintamieji neperrašomi). Raktai – ASP.NET Core konfigūracijos keliai su `__`
vietoje `:`. Šablonas – [`.env.example`](.env.example).

| Raktas | Paskirtis |
|--------|-----------|
| `ConnectionStrings__Default` | PostgreSQL prisijungimo eilutė |
| `Jwt__SigningKey` | JWT parašo raktas, ≥ 32 simboliai (**privalomas ne Development aplinkoje**) |
| `Jwt__AccessTokenMinutes`, `Jwt__RefreshTokenDays` | Žetonų galiojimas (numatytai 15 min / 30 d.) |
| `Cors__AllowedOrigins` | Frontend kilmės, atskirtos kableliais |
| `PasswordReset__ResetUrl` | Frontend puslapis, į kurį veda atkūrimo laiško nuoroda |
| `Swagger__Enabled` | Įjungti Swagger UI ne Development aplinkoje |
| `RateLimiting__AuthPermitsPerMinute` | `/api/auth/*` užklausų limitas vienam IP (numatytai 20/min) |

Nustatymai validuojami paleidžiant – pvz. per trumpas `Jwt:SigningKey` neleidžia API startuoti.

## Sveikatos patikros

| Kelias | Paskirtis |
|--------|-----------|
| `/health/live` | Liveness – procesas veikia (be priklausomybių) |
| `/health/ready` | Readiness – pasiekiama DB |
| `/health` | Visos patikros |

Atsakymas – JSON (`status`, `checks`); nesveikas – HTTP 503.

## Autentifikacija

- Registracija sukuria vartotoją **ir jo namų ūkį** (vartotojas tampa `Owner`). Vienas vartotojas – vienas namų ūkis.
- Slaptažodžiai maišomi `PasswordHasher` (PBKDF2). Minimalus ilgis – 8 simboliai.
- Prisijungus grąžinamas trumpalaikis **JWT prieigos žetonas** (`Authorization: Bearer ...`) ir ilgalaikis
  **atnaujinimo žetonas**. DB saugomos tik atnaujinimo / atkūrimo žetonų SHA-256 maišos.
- `refresh` rotuoja žetoną; pakartotinai pateikus jau panaudotą žetoną atšaukiamos visos vartotojo sesijos.
- `logout` atšaukia atnaujinimo žetoną; prieigos žetonas galioja iki savo pabaigos (≤ 15 min).
- `forgot-password` visada grąžina 202 (neatskleidžia, ar paskyra yra). Nuoroda galioja 60 min., vienkartinė;
  pakeitus slaptažodį atjungiamos visos sesijos. **Laiškai kol kas tik rašomi į žurnalą** (`LoggingEmailSender`) –
  tikras siuntėjas bus prijungtas vėliau.
- Namų ūkio ir sutarčių maršrutai reikalauja prisijungimo ir leidžia prieigą tik prie savo namų ūkio
  (svetimas → 404). Namų ūkį ištrinti gali tik `Owner`.

## Migracijos

```bash
cd backend
dotnet ef migrations add <Pavadinimas> -p src/SaskaituAgentas.Api -o Data/Migrations
dotnet ef database update -p src/SaskaituAgentas.Api
```

## API (v0)

| Metodas | Kelias | Aprašymas |
|---------|--------|-----------|
| POST | `/api/auth/register` | Registracija (+ namų ūkis), grąžina žetonus |
| POST | `/api/auth/login` | Prisijungimas |
| POST | `/api/auth/refresh` | Naujas prieigos žetonas |
| POST | `/api/auth/logout` | Atsijungimas |
| POST | `/api/auth/forgot-password` | Slaptažodžio atkūrimo nuoroda |
| POST | `/api/auth/reset-password` | Naujas slaptažodis pagal nuorodos žetoną |
| GET | `/api/auth/me` | Prisijungęs vartotojas 🔒 |
| GET | `/api/households/me` | Savo namų ūkis 🔒 |
| GET/PUT/DELETE | `/api/households/{id}` | Namų ūkis 🔒 |
| GET/POST | `/api/households/{id}/contracts` | Namų ūkio sutartys 🔒 |
| GET/PUT/DELETE | `/api/households/{id}/contracts/{contractId}` | Viena sutartis 🔒 |

🔒 – reikia `Authorization: Bearer <accessToken>`. Klaidos grąžinamos kaip Problem Details (RFC 9457).

Sutarčių kategorijos: `Electricity`, `Mobile`, `Internet`, `Tv`, `Subscription`.
