namespace SupermarketDesktop.Models;

public sealed class PurchaseSummary
{
    public decimal Subtotal { get; init; }
    public decimal Discount { get; init; }
    public decimal Tax { get; init; }
    public decimal Total => Subtotal - Discount + Tax;
}
