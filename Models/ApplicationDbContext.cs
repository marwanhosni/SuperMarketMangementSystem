using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SuperMarketMangementSystem.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext():base()
        { 
        }
        public ApplicationDbContext(DbContextOptions Option ):base(Option)
        {
        }
        public DbSet<Admin> admins {  get; set; }
        public DbSet<Cashier> cashiers { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<CartItem> cartItems { get; set; }


    }
}
