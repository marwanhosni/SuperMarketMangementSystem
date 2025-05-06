namespace SuperMarketMangementSystem.Models.Repo.Implementation
{
    public class ProductRepo : IProductRepo
    {
        private readonly ApplicationDbContext context;

        public ProductRepo(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task AddAsync(Products products)
        {
            await context.Products.AddAsync(products);
        }
        

        public async Task UpdateAsync(Products product)
        {
            var existingProduct = await context.Products.FindAsync(product.Id);
            if (existingProduct != null)
            {
                context.Entry(existingProduct).CurrentValues.SetValues(product);
            }
            else
            {
                context.Products.Update(product);
            }
        }

        public async Task<List<Products>> GetAllProductsAsync()
        {
            return await context.Products
                .Include(p => p.category)
                .Include(p => p.Discount)
                .ToListAsync();
        }

        public async Task<Products> GetProductByIdAsync(int id)
        {
            return await context.Products
                .AsNoTracking()
                .Include(p => p.Discount)
                .Include(p => p.category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task DeleteAsync(int Id)
        {
            var product = await GetProductByIdAsync(Id);
            context.Remove(product);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
