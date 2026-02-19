using Dapper;
using Microsoft.Data.SqlClient;
using SupermarketDesktop.Models;

namespace SupermarketDesktop.Data;

public sealed class DapperProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public DapperProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<Product>> SearchProductsAsync(string? query, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP 60
                Id,
                Name,
                Sku,
                UnitPrice,
                AvailableQuantity
            FROM Products
            WHERE (@Query IS NULL OR Name LIKE CONCAT('%', @Query, '%') OR Sku LIKE CONCAT('%', @Query, '%'))
            ORDER BY Name;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var products = await connection.QueryAsync<Product>(
            new CommandDefinition(sql, new { Query = string.IsNullOrWhiteSpace(query) ? null : query.Trim() }, cancellationToken: cancellationToken));

        return products.ToList();
    }
}
