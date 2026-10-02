using AppMVVM_1ra.Models;
using AppMVVM_1ra.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliJ.Lang.Annotations;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppMVVM_1ra.ViewModels
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        [ObservableProperty]
        private bool isBusy;

        // Estado para controlar cuándo mostrar el diseño de "Carrito Vacío"
        [ObservableProperty]
        private bool isEmpty = true;

        // El gran total a pagar
        [ObservableProperty]
        private decimal totalAmount;

        public ObservableCollection<CartItem> CartItems { get; } = new();

        // Inyectamos ambos repositorios
        public CartViewModel(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        [RelayCommand]
        public async Task LoadCartAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // 1. Obtenemos lo que hay en SQLite (Ids y Cantidades)
                var items = await _cartRepository.GetCartItemsAsync();

                CartItems.Clear();

                foreach (var item in items)
                {
                    // 2. Hidratamos el modelo: Buscamos el detalle del producto en la API 
                    // para tener el Título y la Imagen que no guardamos en SQLite
                    var productDetail = await _productRepository.GetProductByIdAsync(item.ProductId);

                    // Solo lo agregamos si el producto aún existe en el catálogo
                    if (productDetail != null)
                    {
                        item.ProductInfo = productDetail;
                        CartItems.Add(item);
                    }
                }

                RecalculateState();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task RemoveItemAsync(CartItem item)
        {
            if (item == null) return;

            // Eliminamos de la base de datos
            await _cartRepository.RemoveCartItemAsync(item.ProductId);

            // Eliminamos de la lista en memoria (actualiza la UI al instante)
            CartItems.Remove(item);

            RecalculateState();
        }

        [RelayCommand]
        public async Task ClearCartAsync()
        {
            await _cartRepository.ClearCartAsync();
            CartItems.Clear();

            RecalculateState();
        }

        // Método centralizado para recalcular totales y estados
        private void RecalculateState()
        {
            // LINQ hace que sumar los subtotales sea una sola línea de código
            TotalAmount = CartItems.Sum(x => x.Subtotal);

            // Si la cuenta es cero, IsEmpty es true
            IsEmpty = CartItems.Count == 0;
        }
    }
}