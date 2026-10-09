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

        public ProductRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<Product>>(json);

                    if (products != null && products.Count > 0)
                        return products;
                }

                return GetFallbackProducts();
            }
            catch (Exception)
            {
                return GetFallbackProducts();
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

        private List<Product> GetFallbackProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Title = "Laptop Gamer de Prueba",
                    Price = 750.00m,
                    Category = "Tecnología",
                    Description = "Computadora portátil para desarrollo y diseño.",
                    ImageUrl = "https://fakestoreapi.com/img/81fPKd-2AYL._AC_SL1500_.jpg"
                },
                new Product
                {
                    Id = 2,
                    Title = "Camiseta Casual Deportiva",
                    Price = 25.99m,
                    Category = "Ropa",
                    Description = "Camiseta cómoda de algodón.",
                    ImageUrl = "https://fakestoreapi.com/img/71-3HjGNDUL._AC_SY879._SX._UX._SY._UY_.jpg"
                }
            };
        }
    }
}