public class ProductCategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Enter Valid Name")]
    [MinLength(2, ErrorMessage = "Enter Name Larger Than 2 Letters")]
    public string Name { get; set; } = string.Empty;

    public float Price { get; set; }
    public int Quantity { get; set; }

    public IFormFile? Image { get; set; }  

    public string? OldImage { get; set; }  

    public int CategryId { get; set; }

    public List<Category>? categoriesList { get; set; }
}