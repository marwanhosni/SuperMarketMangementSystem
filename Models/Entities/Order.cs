namespace SuperMarketMangementSystem.Models.Entities;

public class Order
{
    public int Id { get; set; }

    
    public string UserId { get; set; }

    
    public DateTime OrderDate { get; set; } = DateTime.Now;

    
    public decimal TotalPrice { get; set; }

    
    public string Status { get; set; } = "Pending"; // Pending, Delivered, Cancelled

    public ApplicationUser User { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
