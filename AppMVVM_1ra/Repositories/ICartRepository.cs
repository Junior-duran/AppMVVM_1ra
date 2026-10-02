using System.Collections.Generic;
using System.Threading.Tasks;
using AppMVVM_1ra.Models;

namespace AppMVVM_1ra.Repositories
{
    public interface ICartRepository
    {
        Task<List<CartItem>> GetCartItemsAsync();
        Task AddOrUpdateCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(int productId);
        Task ClearCartAsync();
    }
}