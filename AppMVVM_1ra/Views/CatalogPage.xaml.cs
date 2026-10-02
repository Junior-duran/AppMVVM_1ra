using Microsoft.Maui.Controls;
using AppMVVM_1ra.ViewModels;

namespace AppMVVM_1ra.Views
{
    public partial class CatalogPage : ContentPage
    {
        private readonly CatalogViewModel _viewModel;

        public CatalogPage(CatalogViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            // Conectamos la vista con el cerebro (ViewModel)
            BindingContext = _viewModel;
        }

        // Este evento se dispara justo cuando la pantalla se muestra al usuario
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Si la lista está vacía, pedimos que se cargue
            if (_viewModel.Products.Count == 0)
            {
                _viewModel.LoadProductsCommand.Execute(null);
            }
        }
    }
}