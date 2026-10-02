using System.Collections.Generic;
using System.Threading.Tasks;
using AppMVVM_1ra.Models;

namespace AppMVVM_1ra.Repositories
{
    public interface IProductRepository
    {
        // Obtiene todo el catálogo
        Task<IEnumerable<Product>> GetProductsAsync();

        // Obtiene el detalle de un solo producto
        Task<Product?> GetProductByIdAsync(int id);
    }
}