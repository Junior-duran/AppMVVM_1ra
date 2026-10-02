using AppMVVM_1ra.Models;
using AppMVVM_1ra.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliJ.Lang.Annotations;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AppMVVM_1ra.ViewModels
{
    // Heredar de ObservableObject es lo que hace la magia de notificar a la UI
    public partial class CatalogViewModel : ObservableObject
    {
        private readonly IProductRepository _repository;

        // El generador de código creará automáticamente una propiedad pública 'IsBusy'
        [ObservableProperty]
        private bool isBusy;

        // Propiedad para mostrar mensajes de error amigables al usuario
        [ObservableProperty]
        private string errorMessage = string.Empty;

        // ObservableCollection notifica a la UI automáticamente cuando se añaden o quitan elementos
        public ObservableCollection<Product> Products { get; } = new();

        // Inyección de dependencias: el ViewModel pide el repositorio, no lo crea él mismo
        public CatalogViewModel(IProductRepository repository)
        {
            _repository = repository;
        }

        // RelayCommand convierte este método en un comando que un botón puede ejecutar directamente
        [RelayCommand]
        public async Task LoadProductsAsync()
        {
            // Evita ejecuciones duplicadas si el usuario presiona el botón varias veces rápido
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Llamamos a la API a través del repositorio
                var products = await _repository.GetProductsAsync();

                // Limpiamos la lista antes de llenarla de nuevo
                if (Products.Count != 0)
                    Products.Clear();

                foreach (var product in products)
                {
                    Products.Add(product);
                }

                // Manejo de estado vacío si la API falló y el repositorio devolvió una lista vacía
                if (Products.Count == 0)
                {
                    ErrorMessage = "No pudimos cargar el catálogo. Verifica tu conexión.";
                }
            }
            finally
            {
                // El bloque finally asegura que IsBusy siempre se apague, incluso si hay un crash interno
                IsBusy = false;
            }
        }
    }
}