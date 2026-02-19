namespace SupermarketDesktop.Models;

public sealed class Product
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int AvailableQuantity { get; init; }
}
