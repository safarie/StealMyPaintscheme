```
 ██████╗████████╗███████╗ █████╗ ██╗     
██╔════╝╚══██╔══╝██╔════╝██╔══██╗██║     
╚█████╗    ██║   █████╗  ███████║██║     
 ╚═══██╗   ██║   ██╔══╝  ██╔══██║██║     
██████╔╝   ██║   ███████╗██║  ██║███████╗
╚═════╝    ╚═╝   ╚══════╝╚═╝  ╚═╝╚══════╝

███╗   ███╗██╗   ██╗    ██████╗  █████╗ ██╗███╗  ██╗████████╗███████╗ ██████╗██╗  ██╗███████╗███╗   ███╗███████╗
████╗ ████║╚██╗ ██╔╝    ██╔══██╗██╔══██╗██║████╗ ██║╚══██╔══╝██╔════╝██╔════╝██║  ██║██╔════╝████╗ ████║██╔════╝
██╔████╔██║ ╚████╔╝     ██████╔╝███████║██║██╔██╗██║   ██║   ███████╗██║     ███████║█████╗  ██╔████╔██║█████╗  
██║╚██╔╝██║  ╚██╔╝      ██╔═══╝ ██╔══██║██║██║╚████║   ██║   ╚════██║██║     ██╔══██║██╔══╝  ██║╚██╔╝██║██╔══╝  
██║ ╚═╝ ██║   ██║       ██║     ██║  ██║██║██║ ╚███║   ██║   ███████║╚██████╗██║  ██║███████╗██║ ╚═╝ ██║███████╗
╚═╝     ╚═╝   ╚═╝       ╚═╝     ╚═╝  ╚═╝╚═╝╚═╝  ╚══╝   ╚═╝   ╚══════╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝╚══════╝
```

<div align="center">

**Browse. Steal. Conquer the Battlefield.**

*A community platform for Warhammer miniature painters to share, discover, and shamelessly steal each other's paint schemes.*

---

![Angular](https://img.shields.io/badge/Angular-21-DD0031?style=for-the-badge&logo=angular)
![.NET](https://img.shields.io/badge/.NET-9-512BD4?style=for-the-badge&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?style=for-the-badge&logo=typescript)
![TailwindCSS](https://img.shields.io/badge/Tailwind-4-06B6D4?style=for-the-badge&logo=tailwindcss)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=for-the-badge&logo=jsonwebtokens)

</div>

---

## What is this?

You spend three hours perfecting the most god-tier Ultramarine blue recipe — layering, highlighting, wet blending. Then you post it here so everyone else can steal it. That's the point. That's the fun.

**StealMyPaintscheme** is a full-stack web app for the Warhammer hobby community. Create step-by-step paint guides for your miniatures, browse what other painters are cooking, and one-click steal any scheme that catches your eye. Check your paint inventory to see if you've already got everything you need before you even open a pot.

---

## Features

### The Gallery
Browse every paint scheme in the community — yours always bubble to the top. Search by name, description, or tag. Each card shows a **"Ready to Paint"** badge if your inventory already covers all the required paints, or tells you exactly how many pots you're missing.

### Steal Mode
See a scheme you like? Hit **Steal** and it's yours. Clone any community scheme straight into your own collection and make it your own.

### Your Paint Schemes
Full CRUD for your own recipes. Build a scheme step by step — define the surface area, the colour, the technique, and link it to a specific paint from your inventory or the global database.

### Inventory Tracker
Track every paint you own. The app cross-references your inventory against scheme requirements in real time, so you always know whether you can start painting right now or need a trip to the hobby shop.

### Global Paint Database
Over 500+ Warhammer paints pre-loaded from every major range (Citadel, Vallejo, Army Painter and more). Import your collection in seconds — no manual entry required.

---

## Tech Stack

```
Frontend                    Backend                     Database
─────────────────────────   ──────────────────────────  ───────────────
Angular 21                  ASP.NET Core (.NET 9)       PostgreSQL
TypeScript 5.9              Minimal API                 Entity Framework Core
Tailwind CSS v4             JWT Bearer Auth             EF Migrations
RxJS 7.8                    Npgsql Driver
Vitest                      Docker
```

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [PostgreSQL 15+](https://www.postgresql.org)
- [Docker](https://www.docker.com) *(optional)*

### 1. Clone

```bash
git clone https://github.com/your-username/StealMyPaintscheme.git
cd StealMyPaintscheme
```

### 2. Configure the Backend

Edit `backend/StealMyPaintscheme/StealMyPaintscheme.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=stealmypaintscheme;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-chars",
    "Issuer": "StealMyPaintscheme",
    "Audience": "StealMyPaintscheme"
  }
}
```

### 3. Run Database Migrations

```bash
cd backend/StealMyPaintscheme/StealMyPaintscheme.Api
dotnet ef database update
```

### 4. Start the Backend

```bash
dotnet run
```

> The backend automatically spins up the Angular dev server on startup in development mode.

### 5. Start the Frontend (manual)

```bash
cd frontend
npm install
npm start
```

App is live at `http://localhost:4200`

---

## API Reference

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/login` | — | Get a JWT token |
| `POST` | `/users` | — | Register a new account |
| `GET` | `/paint-schemes` | Optional | Browse all community schemes |
| `POST` | `/paint-schemes` | Required | Create a new scheme |
| `PUT` | `/paint-schemes/{id}` | Required | Update your scheme |
| `DELETE` | `/paint-schemes/{id}` | Required | Delete your scheme |
| `GET` | `/inventory-items` | Required | Get your paint inventory |
| `POST` | `/inventory-items` | Required | Add a paint to inventory |
| `PUT` | `/inventory-items/{id}` | Required | Update quantity |
| `DELETE` | `/inventory-items/{id}` | Required | Remove from inventory |
| `GET` | `/global-paints` | — | Browse the full paint database |
| `POST` | `/global-paints/import` | Admin | Bulk-import paints |

OpenAPI spec available at `/openapi/v1.json` in development.

---

## Project Structure

```
StealMyPaintscheme/
├── backend/
│   └── StealMyPaintscheme/
│       └── StealMyPaintscheme.Api/
│           ├── Models/          # PaintScheme, Paint, Step, User, InventoryItem
│           ├── Data/            # EF DbContext
│           ├── Migrations/      # Database migrations
│           ├── Program.cs       # All API endpoints (Minimal API)
│           └── Dockerfile
└── frontend/
    └── src/
        └── app/
            ├── components/
            │   ├── landing/         # Home page
            │   ├── login/           # Login form
            │   ├── register/        # Registration form
            │   ├── paint-schemes/   # Community scheme browser + steal
            │   ├── my-paint-schemes/# Your schemes + editor
            │   └── inventory/       # Paint inventory manager
            └── services/
                ├── auth.service.ts
                ├── paint-scheme.service.ts
                └── inventory.service.ts
```

---

## Docker

```bash
cd backend/StealMyPaintscheme
docker build -f StealMyPaintscheme.Api/Dockerfile -t stealmypaintscheme-api .
docker run -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="..." \
  -e Jwt__Key="..." \
  stealmypaintscheme-api
```

---

## Contributing

1. Fork it
2. Make it better
3. Open a PR
4. We'll steal your ideas

---

## License

MIT — steal freely.
