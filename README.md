# SME Retail & Inventory

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

## Features
- Multi-currency: USD (base)
- Average-cost inventory valuation
- Fiscalisation queue (offline-first)
- Payment methods: Cash, Bank Transfer, Card
