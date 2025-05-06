namespace SuperMarketMangementSystem.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal OriginalPrice { get; set; } 
        public decimal Price { get; set; } 
        public int Quantity { get; set; }
    }
}
