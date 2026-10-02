using System.Text.Json.Serialization;

namespace AppMVVM_1ra.Models
{
    public class Product
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public string ImageUrl { get; set; } = string.Empty;

        // Propiedad calculada útil para la UI que no viene de la API
        [JsonIgnore]
        public string DisplayPrice => $"${Price:F2}";
    }
}