# SąskaitųAgentas.lt: product idea

> A personal agent that keeps cutting your household bills.
> Opportunity score: **9.2/10**

This file describes what the project is and why it exists. Read it before building anything.

---

## One-line summary

A service for Lithuanian households that watches all of a household's contracts (electricity, mobile, internet, TV, subscriptions), finds cheaper alternatives, and then **makes the switch for the customer** with their authorisation. It also tracks renewal dates and checks prices again before each contract renews.

The main button is not "Compare offers". It is **"Sutvarkyk už mane"** ("Sort it out for me").

---

## Why now: evidence from abroad

- **Nous** (UK) raised **$17M** in new funding on 26 October 2026.
- Nous says almost **250,000 UK households** use its household agent.
- It manages more than **$900M** of household spending across energy, broadband, mobile and car insurance.
- Nous is more than a price comparison site. It:
  - monitors bills and renewal dates
  - moves customers to cheaper offers
  - cancels subscriptions
  - flags expensive contract terms
  - handles other household admin

At Nous's scale, this is strong evidence that the model works.

---

## The Lithuanian version

A family connects or uploads:

- electricity bill
- mobile contracts
- home internet
- TV
- *(later)* insurance
- *(later)* subscriptions

### What the user sees

**Current household contracts**

| Contract  | Cost / month |
|-----------|-------------:|
| Electricity | €67.00 |
| Mobile #1   | €22.00 |
| Mobile #2   | €19.00 |
| Internet    | €24.99 |
| TV          | €12.99 |

**Possible savings**

| Change | Saving |
|--------|-------:|
| Mobile #1 → alternative plan | −€8/month |
| Internet → same speed, cheaper | −€7/month |
| Electricity → alternative plan based on consumption | −€6/month |

**Potential annual saving: €252**

Then: **[ Sutvarkyk už mane ]**. With the customer's authorisation, the service carries out the change, tracks when fixed-term contracts expire and checks again before renewal.

**The ongoing relationship is the business.** One-off comparison is not.

---

## Competition in Lithuania

Lithuania already has plenty of single-category comparison tools:

| Category | Existing player |
|----------|-----------------|
| Electricity prices | **VERT**, the official electricity-price comparison tool |
| Mobile plans | **RysioPlanai** |
| Home internet | **VisiPlanai** (data updated as recently as 27 Oct) |
| Insurance | **Draudykle.lt** |
| Bill payment aggregation | **Elektroninių mokėjimų agentūra** (owned by Ignitis): collects payments for electricity, gas, water, heating, communications and waste |

**Price comparison is NOT the opportunity.** Our research did not find a strong Lithuanian service whose job is to:

> continuously monitor all of a household's contracts and actively help switch, cancel or renegotiate them.

That gap is the opportunity. The comparison data already exists in pieces. Nobody provides the layer that **acts on it** for the household on an ongoing basis.

---

## Scope

### Version 1 (no insurance)
- Electricity
- Mobile
- Internet
- TV / subscriptions

### Later
- Insurance, through a **licensed broker** partner. Selling insurance ourselves would bring regulated intermediation rules into play, so add it only after demand is proven.

---

## Business model (test two models at once)

1. **Membership:** €4.99–€7.99 per household per month.
2. **Savings share:** about 15–25% of the verified saving in the first year.

A variant that could be attractive:
- **Free until we find €100+ of annual savings**, then convert the household to a subscription.

Later:
- **Referral commissions from providers.** Be fully transparent about these. Customers must trust that recommendations are based on their savings, not on which provider pays the highest commission.

---

## Target customer

The target is **busy households**, not people who already enjoy comparing prices. In particular, households with:

- 2–4 mobile contracts
- an electricity contract
- internet/TV
- several subscriptions
- insurance renewals

The more complex the household, the more valuable the service.

---

## MVP plan

Don't integrate with every provider at the start.

1. Sign up **50 households**.
2. Ask them to forward or upload their latest bills for:
   - electricity
   - internet
   - mobile
3. Work out cheaper alternatives, partly by hand and partly automatically.
4. Tell each household: *"I found €186/year of savings. Do you want us to handle the change?"*

### The key metric

What matters most is not how much saving we find. It is:

> **What percentage of people let us act for them?**

### Go / no-go threshold

Build the product if the 50 households show roughly:

- median saving of **€150/year**
- **60%** accept at least one switch
- **30%** agree to pay **€5/month** for ongoing monitoring

---

## Effort & risk

- **Capital:** €3k–€10k
- **Difficulty:** Medium

### Main risks
- Lithuania is a small market.
- Telecom prices are already competitive.
- Provider integrations and customer authorisations can be painful.
- Users must trust us with sensitive account information.
- Providers could make switching difficult.

---

## Why 9.2/10

- Nous's recent scale is unusually strong evidence that the model works.
- Lithuania already has all the scattered comparison data needed to build a first version.
- The missing piece is an **agent that acts for the household and stays with it over time**: it monitors, switches, cancels and re-checks before renewal. Lithuania has comparison tools, but nothing that does this.

---

## Repository structure

| Directory | Contents |
|-----------|----------|
| `frontend/` | Next.js (App Router), TypeScript, Tailwind CSS v4, shadcn/ui |
| `backend/` | ASP.NET Core Web API (.NET 10), EF Core + PostgreSQL. See `backend/README.md` |
| `infrastructure/` | Local Docker Compose (PostgreSQL); later IaC, deployment, CI/CD |

npm workspaces are used. From the repository root:

```bash
npm install
npm run dev
```
