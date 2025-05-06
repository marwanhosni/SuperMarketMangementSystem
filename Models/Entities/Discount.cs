namespace SuperMarketMangementSystem.Models.Entities;

public class Discount
{
    public int Id { get; set; }

    public string Name { get; set; }

    [Range(0, 100, ErrorMessage = "The Amount Of Discount Between 0 To 100")]
    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? ProductId { get; set; }
    public int? CategoryId { get; set; }

    [ForeignKey("ProductId")]
    public Products? Product { get; set; }

    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }
}
