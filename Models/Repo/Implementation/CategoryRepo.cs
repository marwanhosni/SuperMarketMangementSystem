namespace SuperMarketMangementSystem.Models.Repo.Implementation
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly ApplicationDbContext context;

        public CategoryRepo(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task AddCategoryAsync(Category category)
        {
            await context.categories.AddAsync(category);
            await SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            context.categories.Update(category);
            await SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await context.categories
                .Include(p => p.Products)
                .ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await context.categories
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                context.categories.Remove(category);
                await SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
