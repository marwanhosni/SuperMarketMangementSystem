namespace SuperMarketMangementSystem.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; } = string.Empty;

        public string AnotherPhoneNumber { get; set; } = string.Empty;
    }
}
