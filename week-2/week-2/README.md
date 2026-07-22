# Week 2 - EF Core 8.0, ASP.NET Core Web API, React

This folder contains a complete Retail Inventory hands-on implementation for the week-2 module.

## Projects

- `RetailInventory.Api` - ASP.NET Core 8 Web API using EF Core 8 and SQLite.
- `RetailInventory.Client` - React + Vite inventory dashboard.
- `EF Core 8.0 HOL.txt` - extracted text from the provided PDF lab file.

## Covered Lab Concepts

- ORM with EF Core models and DbContext.
- Categories and products one-to-many relationship.
- Product detail one-to-one relationship.
- Product and tag many-to-many relationship.
- EF Core migrations and seeded data.
- CRUD endpoints using async EF Core APIs.
- LINQ filtering, projection, ordering, DTO responses, eager loading, and explicit loading.
- `AsNoTracking`, compiled queries, batch updates with `ExecuteUpdateAsync`, and concurrency handling with `RowVersion`.
- React frontend that consumes the Web API.

## Run Backend

```bash
cd DeepSkilling/week-2
dotnet tool restore
dotnet tool run dotnet-ef database update --project RetailInventory.Api/RetailInventory.Api.csproj --startup-project RetailInventory.Api/RetailInventory.Api.csproj --context AppDbContext
dotnet RetailInventory.Api/bin/Debug/net8.0/RetailInventory.Api.dll
```

Backend URL:

```text
http://localhost:5058
```

Useful endpoints:

```text
GET  /api/products
GET  /api/categories
GET  /api/products/reports/expensive?minimumPrice=10000
POST /api/products/batch/restock?quantity=10
```

## Run React Client

```bash
cd DeepSkilling/week-2/RetailInventory.Client
npm install
npm run dev
```

Client URL:

```text
http://127.0.0.1:5173/
```
