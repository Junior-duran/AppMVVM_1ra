using SQLite;

namespace AppMVVM_1ra.Models
{
    public class CartItem
    {
        // Definimos el ProductId como la clave primaria. 
        // Así aseguramos que un producto no se duplique en el carrito (solo se actualiza su cantidad)
        [PrimaryKey]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtTimeOfAdding { get; set; }

        // [Ignore] es CRUCIAL. SQLite no sabe cómo guardar un objeto complejo (Product) dentro de una tabla.
        // Con esto le decimos que ignore esta propiedad al guardar, la usaremos solo en memoria para la Vista.
        [Ignore]
        public Product? ProductInfo { get; set; }

        public decimal Subtotal => Quantity * PriceAtTimeOfAdding;
    }
}