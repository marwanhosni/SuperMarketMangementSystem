namespace SuperMarketMangementSystem.Models.Repo.Implementation
{
    public class DiscountServices : IDiscountServices
    {
        private readonly ApplicationDbContext context;

        public DiscountServices(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task AddAsync(Discount discount)
        {
            await context.Discounts.AddAsync(discount);
        }

        public async Task UpdateAsync(Discount discount)
        {
            var existingDiscount = await context.Discounts.FindAsync(discount.Id);
            if (existingDiscount != null)
            {
                context.Entry(existingDiscount).CurrentValues.SetValues(discount);
            }
            else
            {
                context.Discounts.Update(discount);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var discount = await context.Discounts
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discount != null)
            {
                var products = await context.Products
                    .Where(p => p.DiscountId == id)
                    .ToListAsync();

                foreach (var product in products)
                {
                    product.DiscountId = null;
                    context.Products.Update(product);
                }

                var discountToRemove = new Discount { Id = id };
                context.Discounts.Remove(discountToRemove);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Discount>> GetAllAsync()
        {
            return await context.Discounts
                .Include(d => d.Product)
                .Include(d => d.Category)
                .ToListAsync();
        }

        public async Task<Discount> GetByIdAsync(int id)
        {
            return await context.Discounts
                .AsNoTracking()
                .Include(d => d.Product)
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<decimal?> GetApplicableDiscountAsync(int productId, int categoryId)
        {
            var productDiscount = await context.Discounts
                .Where(d => d.ProductId == productId &&
                            d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                .OrderByDescending(d => d.DiscountPercentage)
                .FirstOrDefaultAsync();

            return productDiscount?.DiscountPercentage ?? 0;
        }

        public async Task SyncDiscountsWithProductsAsync(IProductRepo productRepo)
        {
            var discounts = await GetAllAsync();

            var allProducts = await productRepo.GetAllProductsAsync();
            foreach (var product in allProducts)
            {
                product.DiscountId = null;
                await productRepo.UpdateAsync(product);
            }
            await productRepo.SaveChangesAsync();

            foreach (var discount in discounts)
            {
                if (discount.ProductId.HasValue)
                {
                    var product = await productRepo.GetProductByIdAsync(discount.ProductId.Value);
                    if (product != null)
                    {
                        product.DiscountId = discount.Id;
                        await productRepo.UpdateAsync(product);
                    }
                }
            }

            await productRepo.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
