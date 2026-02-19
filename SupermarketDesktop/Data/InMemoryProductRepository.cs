using SupermarketDesktop.Models;

namespace SupermarketDesktop.Data;

public sealed class InMemoryProductRepository : IProductRepository
{
    private static readonly IReadOnlyList<Product> Products =
    [
        new() { Id = 1, Name = "Whole Milk 1L", Sku = "DAI-1001", UnitPrice = 1.45m, AvailableQuantity = 64 },
        new() { Id = 2, Name = "Brown Bread", Sku = "BAK-3002", UnitPrice = 0.99m, AvailableQuantity = 30 },
        new() { Id = 3, Name = "Basmati Rice 5kg", Sku = "GRC-2101", UnitPrice = 7.80m, AvailableQuantity = 14 },
        new() { Id = 4, Name = "Eggs 12-Pack", Sku = "DAI-1008", UnitPrice = 2.35m, AvailableQuantity = 25 },
        new() { Id = 5, Name = "Arabica Coffee 500g", Sku = "BEV-7102", UnitPrice = 5.90m, AvailableQuantity = 19 }
    ];

    public Task<IReadOnlyList<Product>> SearchProductsAsync(string? query, CancellationToken cancellationToken = default)
    {
        var result = string.IsNullOrWhiteSpace(query)
            ? Products
            : Products.Where(p =>
                    p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.Sku.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

        return Task.FromResult(result);
    }
}
