namespace SuperMarketMangementSystem.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Products>? Products { get; set; }
    }

}
