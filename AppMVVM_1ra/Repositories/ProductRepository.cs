using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AppMVVM_1ra.Models;

namespace AppMVVM_1ra.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://fakestoreapi.com/products";

        // Inyectamos el HttpClient
        public ProductRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);

                // Solo procesamos si el servidor respondió con un 200 OK
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<Product>>(json);

                    return products ?? new List<Product>();
                }

                // Si la API falla (ej. error 500), devolvemos una lista vacía 
                // para evitar que la aplicación colapse.
                return new List<Product>();
            }
            catch (Exception)
            {
                // Si no hay internet o falla la red, el try-catch atrapa la excepción
                // Aquí en el futuro se puede conectar con SQLite para devolver la caché local
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Product>(json);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}