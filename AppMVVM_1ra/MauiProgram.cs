using AppMVVM_1ra;
using AppMVVM_1ra.Repositories;
using AppMVVM_1ra.ViewModels;
using AppMVVM_1ra.Views;
using CommunityToolkit.Maui; // Necesario para el toolkit (convertidores de UI)
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace AppMVVM_1ra
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // Inicializa el Toolkit
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
            // AddHttpClient configura HttpClient de forma segura, evitando el agotamiento de sockets
            builder.Services.AddHttpClient<IProductRepository, ProductRepository>();
            // AddSingleton asegura que haya una única instancia del repositorio en toda la app
            builder.Services.AddSingleton<IProductRepository, ProductRepository>();

            // NUEVO: Repositorio del Carrito (Singleton para mantener la conexión SQLite viva)
            builder.Services.AddSingleton<ICartRepository, CartRepository>();

            // ==========================================
            // 2. REGISTRO DE VIEWMODELS
            // ==========================================
            // AddTransient crea una nueva instancia cada vez que se solicita
            builder.Services.AddTransient<CatalogViewModel>();

            // NUEVO: ViewModel del Carrito
            builder.Services.AddTransient<CartViewModel>();

            // ==========================================
            // 3. REGISTRO DE VISTAS (Páginas)
            // ==========================================
            builder.Services.AddTransient<CatalogPage>();

            // NUEVO: Vista del Carrito
            builder.Services.AddTransient<CartPage>();

            return builder.Build();
        }
    }
}