using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using AppMVVM_1ra.Models;

namespace AppMVVM_1ra.Repositories
{
    public class CartRepository : ICartRepository
    {
        private SQLiteAsyncConnection _database;

        // Método privado para asegurar que la base de datos esté lista antes de cualquier operación
        private async Task InitAsync()
        {
            if (_database != null)
                return;

            // Define la ruta donde se guardará el archivo de la base de datos en el teléfono
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CatalogoApp.db3");

            _database = new SQLiteAsyncConnection(databasePath);

            // Crea la tabla si no existe. Si ya existe, no hace nada.
            await _database.CreateTableAsync<CartItem>();
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            await InitAsync();
            return await _database.Table<CartItem>().ToListAsync();
        }

        public async Task AddOrUpdateCartItemAsync(CartItem item)
        {
            await InitAsync();

            // Buscamos si el producto ya está en el carrito
            var existingItem = await _database.Table<CartItem>().FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

            if (existingItem != null)
            {
                // Si existe, sumamos la cantidad
                existingItem.Quantity += item.Quantity;
                await _database.UpdateAsync(existingItem);
            }
            else
            {
                // Si es nuevo, lo insertamos
                await _database.InsertAsync(item);
            }
        }

        public async Task RemoveCartItemAsync(int productId)
        {
            await InitAsync();
            // Borra el registro coincidente usando la llave primaria
            await _database.DeleteAsync<CartItem>(productId);
        }

        public async Task ClearCartAsync()
        {
            await InitAsync();
            await _database.DeleteAllAsync<CartItem>();
        }
    }
}