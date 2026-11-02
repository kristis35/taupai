# backend

Serverio dalis: ASP.NET Core Web API (.NET 10) + PostgreSQL per EF Core (Npgsql).

## Struktūra

```
backend/
├── SaskaituAgentas.slnx
├── dotnet-tools.json                 # lokalus dotnet-ef įrankis
└── src/SaskaituAgentas.Api/
    ├── Controllers/                  # HTTP sluoksnis (ploni kontroleriai)
    ├── Services/                     # verslo logika, ServiceResult
    ├── Repositories/                 # duomenų prieiga per EF Core
    ├── Data/
    │   ├── AppDbContext.cs
    │   ├── Configurations/           # IEntityTypeConfiguration<T>
    │   ├── Migrations/               # EF Core migracijos
    │   └── DesignTimeDbContextFactory.cs
    ├── Models/                       # domeno esybės (Household, Contract)
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
dotnet tool restore
dotnet run --project src/SaskaituAgentas.Api
```

API: <http://localhost:5109>. `Development` aplinkoje migracijos pritaikomos automatiškai paleidžiant
(`Database:MigrateOnStartup`), OpenAPI schema – `/openapi/v1.json`, sveikatos patikra – `/health`.
Užklausų pavyzdžiai – `src/SaskaituAgentas.Api/SaskaituAgentas.Api.http`.

Prisijungimo eilutė: `ConnectionStrings:Default` (`appsettings.Development.json`) arba aplinkos kintamasis
`ConnectionStrings__Default`.

## Migracijos

```bash
cd backend
dotnet ef migrations add <Pavadinimas> -p src/SaskaituAgentas.Api -o Data/Migrations
dotnet ef database update -p src/SaskaituAgentas.Api
```

## API (v0)

| Metodas | Kelias | Aprašymas |
|---------|--------|-----------|
| GET/POST | `/api/households` | Namų ūkių sąrašas / sukūrimas |
| GET/PUT/DELETE | `/api/households/{id}` | Vienas namų ūkis |
| GET/POST | `/api/households/{id}/contracts` | Namų ūkio sutartys |
| GET/PUT/DELETE | `/api/households/{id}/contracts/{contractId}` | Viena sutartis |

Sutarčių kategorijos: `Electricity`, `Mobile`, `Internet`, `Tv`, `Subscription`.
