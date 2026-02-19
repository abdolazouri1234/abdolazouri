# abdolazouri

## Supermarket Purchase Page (WPF + Dapper)

This repository now includes a desktop WPF sample at `SupermarketDesktop/` that provides a modern purchase page for supermarket cashiers.

### Highlights
- Product search panel and catalog grid.
- Purchase cart with quantity editing.
- Live totals for subtotal, discount, tax, and grand total.
- Repository abstraction that supports both:
  - `InMemoryProductRepository` (default demo mode), and
  - `DapperProductRepository` for SQL Server using Dapper.

### Run
```bash
dotnet run --project SupermarketDesktop/SupermarketDesktop.csproj
```

### Dapper setup
Swap the default repository in `PurchasePageViewModel` constructor with `DapperProductRepository` and pass your SQL Server connection string.
