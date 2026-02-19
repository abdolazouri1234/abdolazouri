using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SupermarketDesktop.Data;
using SupermarketDesktop.Models;

namespace SupermarketDesktop.ViewModels;

public sealed class PurchasePageViewModel : INotifyPropertyChanged
{
    private readonly IProductRepository _repository;
    private string _searchTerm = string.Empty;
    private Product? _selectedProduct;
    private CartItem? _selectedCartItem;
    private decimal _taxRate = 0.15m;
    private decimal _discountRate;

    public PurchasePageViewModel()
        : this(new InMemoryProductRepository())
    {
    }

    public PurchasePageViewModel(IProductRepository repository)
    {
        _repository = repository;
        Products = new ObservableCollection<Product>();
        CartItems = new ObservableCollection<CartItem>();

        CartItems.CollectionChanged += (_, _) => RecalculateTotals();

        AddToCartCommand = new RelayCommand(_ => AddSelectedProductToCart(), _ => SelectedProduct is not null);
        RemoveFromCartCommand = new RelayCommand(_ => RemoveSelectedCartItem(), _ => SelectedCartItem is not null);
        CheckoutCommand = new RelayCommand(_ => Checkout(), _ => CartItems.Count > 0);
        SearchCommand = new RelayCommand(async _ => await LoadProductsAsync());

        _ = LoadProductsAsync();
    }

    public ObservableCollection<Product> Products { get; }
    public ObservableCollection<CartItem> CartItems { get; }

    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand CheckoutCommand { get; }
    public ICommand SearchCommand { get; }

    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (_searchTerm == value)
            {
                return;
            }

            _searchTerm = value;
            OnPropertyChanged();
        }
    }

    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (_selectedProduct == value)
            {
                return;
            }

            _selectedProduct = value;
            OnPropertyChanged();
            RaiseCommandState();
        }
    }

    public CartItem? SelectedCartItem
    {
        get => _selectedCartItem;
        set
        {
            if (_selectedCartItem == value)
            {
                return;
            }

            _selectedCartItem = value;
            OnPropertyChanged();
            RaiseCommandState();
        }
    }

    public decimal TaxRate
    {
        get => _taxRate;
        set
        {
            if (_taxRate == value)
            {
                return;
            }

            _taxRate = value;
            OnPropertyChanged();
            RecalculateTotals();
        }
    }

    public decimal DiscountRate
    {
        get => _discountRate;
        set
        {
            if (_discountRate == value)
            {
                return;
            }

            _discountRate = value;
            OnPropertyChanged();
            RecalculateTotals();
        }
    }

    public PurchaseSummary Summary { get; private set; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private async Task LoadProductsAsync()
    {
        var products = await _repository.SearchProductsAsync(SearchTerm);

        Application.Current.Dispatcher.Invoke(() =>
        {
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        });
    }

    private void AddSelectedProductToCart()
    {
        if (SelectedProduct is null)
        {
            return;
        }

        var item = CartItems.FirstOrDefault(ci => ci.Product.Id == SelectedProduct.Id);
        if (item is null)
        {
            item = new CartItem { Product = SelectedProduct, Quantity = 1 };
            item.PropertyChanged += CartItemOnPropertyChanged;
            CartItems.Add(item);
        }
        else
        {
            item.Quantity += 1;
        }

        RecalculateTotals();
        RaiseCommandState();
    }

    private void RemoveSelectedCartItem()
    {
        if (SelectedCartItem is null)
        {
            return;
        }

        SelectedCartItem.PropertyChanged -= CartItemOnPropertyChanged;
        CartItems.Remove(SelectedCartItem);
        SelectedCartItem = null;

        RecalculateTotals();
        RaiseCommandState();
    }

    private void CartItemOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CartItem.Quantity))
        {
            RecalculateTotals();
        }
    }

    private void RecalculateTotals()
    {
        var subtotal = CartItems.Sum(ci => ci.LineTotal);
        var discount = subtotal * DiscountRate;
        var taxableAmount = subtotal - discount;
        var tax = taxableAmount * TaxRate;

        Summary = new PurchaseSummary
        {
            Subtotal = subtotal,
            Discount = discount,
            Tax = tax
        };

        OnPropertyChanged(nameof(Summary));
        RaiseCommandState();
    }

    private void Checkout()
    {
        MessageBox.Show($"Order complete. Grand total: {Summary.Total:C2}", "Checkout", MessageBoxButton.OK, MessageBoxImage.Information);
        foreach (var item in CartItems)
        {
            item.PropertyChanged -= CartItemOnPropertyChanged;
        }

        CartItems.Clear();
        RecalculateTotals();
        RaiseCommandState();
    }

    private void RaiseCommandState()
    {
        (AddToCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RemoveFromCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CheckoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
