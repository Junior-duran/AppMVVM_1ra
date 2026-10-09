using Microsoft.Maui.Controls;
using AppMVVM_1ra.ViewModels;

namespace AppMVVM_1ra.Views
{
    public partial class CartPage : ContentPage
    {
        public CartPage(CartViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Cada vez que la pantalla aparece, le decimos al ViewModel que cargue los datos frescos
            if (BindingContext is CartViewModel viewModel)
            {
                viewModel.LoadCartCommand.Execute(null);
            }
        }
    }
}