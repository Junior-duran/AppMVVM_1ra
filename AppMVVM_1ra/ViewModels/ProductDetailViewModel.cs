using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using AppMVVM_1ra.Models;
using AppMVVM_1ra.Repositories;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace AppMVVM_1ra.ViewModels
{
    [QueryProperty(nameof(ProductId), "id")]
    public partial class ProductDetailViewModel : ObservableObject
    {
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;

        [ObservableProperty]
        private int productId;

        // Le agregamos el signo de interrogación (?) para permitir nulos al inicio y quitar la advertencia
        [ObservableProperty]
        private Product? currentProduct;

        [ObservableProperty]
        private bool isBusy;

        public ProductDetailViewModel(IProductRepository productRepository, ICartRepository cartRepository)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
        }

        partial void OnProductIdChanged(int value)
        {
            LoadProductDetailsAsync(value);
        }

        private async Task LoadProductDetailsAsync(int id)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                CurrentProduct = await _productRepository.GetProductByIdAsync(id);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddToCartAsync()
        {
            if (CurrentProduct == null) return;

            var cartItem = new CartItem
            {
                ProductId = CurrentProduct.Id,
                Quantity = 1,
                PriceAtTimeOfAdding = CurrentProduct.Price
            };

            await _cartRepository.AddOrUpdateCartItemAsync(cartItem);

            try
            {
                var toast = Toast.Make($"🛒 {CurrentProduct.Title} añadido", ToastDuration.Short, 14);
                await toast.Show();
            }
            catch
            {
                // Manejo silencioso
            }
        }
    }
}