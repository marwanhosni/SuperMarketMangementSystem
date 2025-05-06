namespace SuperMarketMangementSystem.ModelView
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        [Required(ErrorMessage = " Enter A Valid  number")]
        public int Quantity { get; set; }
        public string Description{ get; set; }
        public string Image { get; set; }

    }
}
