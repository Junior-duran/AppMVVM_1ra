using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using AppMVVM_1ra.Models;
using AppMVVM_1ra.Repositories;

namespace AppMVVM_1ra.ViewModels
{
    public partial class CatalogViewModel : ObservableObject
    {
        private readonly IProductRepository _repository;
        private readonly ICartRepository _cartRepository;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public ObservableCollection<Product> Products { get; } = new();

        public CatalogViewModel(IProductRepository repository, ICartRepository cartRepository)
        {
            _repository = repository;
            _cartRepository = cartRepository;
        }

        [RelayCommand]
        public async Task LoadProductsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var products = await _repository.GetProductsAsync();

                Products.Clear();

                foreach (var product in products)
                {
                    Products.Add(product);
                }

                if (Products.Count == 0)
                {
                    ErrorMessage = "No pudimos cargar el catálogo. Verifica tu conexión.";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddToCartAsync(Product product)
        {
            if (product == null) return;

            var cartItem = new CartItem
            {
                ProductId = product.Id,
                Quantity = 1,
                PriceAtTimeOfAdding = product.Price
            };

            // 1. Guardamos en la base de datos (Esto funciona impecable)
            await _cartRepository.AddOrUpdateCartItemAsync(cartItem);

            // 2. Intentamos mostrar el mensaje silencioso
            try
            {
                var toast = Toast.Make($"🛒 {product.Title} añadido", ToastDuration.Short, 14);
                await toast.Show();
            }
            catch (Exception)
            {
                // Si Windows bloquea la notificación nativa en modo depuración, 
                // atrapamos el error para que la aplicación no colapse.
                // El producto ya está en el carrito, así que el usuario puede seguir navegando.
            }
        }
        [RelayCommand]
        public async Task GoToCartAsync()
        {
            await Shell.Current.GoToAsync("//CartPage");
        }
    }
}