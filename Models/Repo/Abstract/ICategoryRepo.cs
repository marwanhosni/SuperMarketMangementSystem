namespace SuperMarketMangementSystem.Models.Repo.Abstract
{
    public interface ICategoryRepo
    {
       
        Task AddCategoryAsync(Category category);

        Task UpdateCategoryAsync(Category category);

        Task<List<Category>> GetAllCategoriesAsync();

        Task<Category> GetByIdAsync(int id);

        Task DeleteCategoryAsync(int id);

        Task SaveChangesAsync();
    }
}
