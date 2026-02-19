using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SupermarketDesktop.Models;

public sealed class CartItem : INotifyPropertyChanged
{
    private int _quantity;

    public required Product Product { get; init; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value)
            {
                return;
            }

            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LineTotal));
        }
    }

    public decimal UnitPrice => Product.UnitPrice;
    public decimal LineTotal => UnitPrice * Quantity;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
