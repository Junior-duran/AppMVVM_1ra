using Microsoft.Maui.Controls;
using AppMVVM_1ra.ViewModels;

namespace AppMVVM_1ra.Views
{
    public partial class ProductDetailPage : ContentPage
    {
        public ProductDetailPage(ProductDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}