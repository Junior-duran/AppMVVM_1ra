using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using AppMVVM_1ra.Models;
using AppMVVM_1ra.Repositories;

namespace AppMVVM_1ra.ViewModels
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly ICartRepository _cartRepository;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private decimal subTotal;

        [ObservableProperty]
        private decimal itbis;

        [ObservableProperty]
        private decimal shippingCost = 5.00m;

        [ObservableProperty]
        private decimal grandTotal;

        public ObservableCollection<CartItem> CartItems { get; } = new();

        public CartViewModel(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [RelayCommand]
        public async Task LoadCartAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                var items = await _cartRepository.GetCartItemsAsync();

                CartItems.Clear();
                decimal tempSubTotal = 0;

                foreach (var item in items)
                {
                    CartItems.Add(item);
                    tempSubTotal += (item.PriceAtTimeOfAdding * item.Quantity);
                }

                SubTotal = tempSubTotal;
                Itbis = SubTotal * 0.18m;
                GrandTotal = SubTotal + Itbis + ShippingCost;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task RemoveFromCartAsync(CartItem item)
        {
            if (item == null) return;
            await _cartRepository.RemoveCartItemAsync(item.ProductId);
            await LoadCartAsync();
        }

        [RelayCommand]
        public async Task ProcessPaymentAsync()
        {
            if (CartItems.Count == 0)
            {
                await Shell.Current.DisplayAlert("Error", "El carrito está vacío.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                string fileName = $"Factura_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                CreateInvoiceHtml(filePath);

                foreach (var item in CartItems)
                {
                    await _cartRepository.RemoveCartItemAsync(item.ProductId);
                }

                await LoadCartAsync();

                await Shell.Current.DisplayAlert(
                    "¡Pago Exitoso!",
                    $"Tu pago por {GrandTotal:C} ha sido procesado.\n\nAbriendo factura...",
                    "OK");

                await Launcher.Default.OpenAsync(new OpenFileRequest("Factura", new ReadOnlyFile(filePath)));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CreateInvoiceHtml(string filePath)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head><meta charset='UTF-8'><style>");
            html.AppendLine("body { font-family: 'Segoe UI', Tahoma, sans-serif; padding: 20px; color: #333; }");
            html.AppendLine(".container { max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 30px; border-radius: 10px; }");
            html.AppendLine("h1 { text-align: center; color: #0056D2; }");
            html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            html.AppendLine("th, td { border-bottom: 1px solid #ddd; padding: 12px; text-align: left; }");
            html.AppendLine(".totals { text-align: right; margin-top: 20px; font-size: 16px; }");
            html.AppendLine(".grand-total { font-size: 22px; font-weight: bold; color: #0056D2; }");
            html.AppendLine("</style></head><body>");

            html.AppendLine("<div class='container'>");
            html.AppendLine("<h1>FACTURA DE COMPRA</h1>");
            html.AppendLine($"<p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");
            html.AppendLine("<p><strong>Cliente:</strong> Junior Durán de la Rosa</p>");

            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Producto (ID)</th><th>Cant.</th><th>Precio</th><th>Total</th></tr>");

            foreach (var item in CartItems)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{item.ProductId}</td>");
                html.AppendLine($"<td>{item.Quantity}</td>");
                html.AppendLine($"<td>{item.PriceAtTimeOfAdding:C}</td>");
                html.AppendLine($"<td>{(item.PriceAtTimeOfAdding * item.Quantity):C}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");

            html.AppendLine("<div class='totals'>");
            html.AppendLine($"<p>Subtotal: {SubTotal:C}</p>");
            html.AppendLine($"<p>ITBIS (18%): {Itbis:C}</p>");
            html.AppendLine($"<p>Envío: {ShippingCost:C}</p>");
            html.AppendLine($"<p class='grand-total'>TOTAL A PAGAR: {GrandTotal:C}</p>");
            html.AppendLine("</div>");

            html.AppendLine("</div></body></html>");

            File.WriteAllText(filePath, html.ToString());
        }
    }
}