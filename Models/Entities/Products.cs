namespace SuperMarketMangementSystem.Models.Entities
{
    public class Products
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter Name")]
        [MaxLength(30, ErrorMessage = ("The Name Should Be Less Than 30 Letters"))]
        [MinLength(1, ErrorMessage = ("The Should Be Larger Than One Letter "))]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = ("Enter A Valid Price In Egyptian Pound"))]
        public float Price { get; set; }

        [Required(ErrorMessage = ("Enter A Valid Quantity Of Products"))]
        public int Quantity { get; set; }

        [Required(ErrorMessage = ("Select the Valid Category Of The Products"))]
        [ForeignKey("category")]
        public int CategoryId { get; set; }

        [RegularExpression(@"^.*\.(jpg|jpeg|png)$", ErrorMessage = "Please upload a valid image file (.jpg, .jpeg, .png)")]
        public string? Image { get; set; } = string.Empty;

        public Category category { get; set; } = default!;

        // جديد: خصم مرتبط بالمنتج
        [ForeignKey("Discount")]
        public int? DiscountId { get; set; } // إذا كان المنتج له خصم (بالتالى يكون nullable)
        public Discount? Discount { get; set; } // لربط الخصم بالمنتج
    }
}
