using SupermarketDesktop.Models;

namespace SupermarketDesktop.Data;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> SearchProductsAsync(string? query, CancellationToken cancellationToken = default);
}
