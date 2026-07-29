# SME Retail & Inventory (Zimbabwe)

Single-project ASP.NET Core backend with MySQL, Meilisearch, multi-currency, VAT, and average-cost inventory.

## Quick Start

```bash
docker compose up --build
```

- API: http://localhost:5000
- Frontend: http://localhost:3000
- Meilisearch: http://localhost:7700

## Stack
- **Backend**: ASP.NET Core 8 (single project)
- **Database**: MySQL 8 + EF Core (Pomelo)
- **Search**: Meilisearch
- **Frontend**: React + Vite + TypeScript

## Zimbabwe Features
- Multi-currency: USD (base) + ZiG
- Average-cost inventory valuation
- ZIMRA fiscalisation queue (offline-first)
- Mobile money methods: ECOCASH, ZIPIT, INNWALLET, OMARI
