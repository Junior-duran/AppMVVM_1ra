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
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel.Products.Count == 0)
            {
                _viewModel.LoadProductsCommand.Execute(null);
            }
        }
    }
}