using AppMVVM_1ra;
using AppMVVM_1ra.Repositories;
using AppMVVM_1ra.ViewModels;
using AppMVVM_1ra.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AppMVVM_1ra
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif

            // ==========================================
            // 1. REGISTRO DE SERVICIOS CORE Y REPOSITORIOS
            // ==========================================
            builder.Services.AddSingleton<IProductRepository>(sp => new ProductRepository(new System.Net.Http.HttpClient()));
            builder.Services.AddSingleton<ICartRepository, CartRepository>();

            // ==========================================
            // 2. REGISTRO DE VIEWMODELS
            // ==========================================
            builder.Services.AddTransient<CatalogViewModel>();
            builder.Services.AddTransient<CartViewModel>();

            // Nueva línea agregada para el cerebro de los detalles
            builder.Services.AddTransient<ProductDetailViewModel>();

            // ==========================================
            // 3. REGISTRO DE VISTAS (Páginas)
            // ==========================================
            builder.Services.AddTransient<CatalogPage>();
            builder.Services.AddTransient<CartPage>();

            // Nueva línea agregada para la vista de los detalles
            builder.Services.AddTransient<ProductDetailPage>();

            return builder.Build();
        }
    }
}