namespace SuperMarketMangementSystem.Models.Repo.Abstract
{
    public interface IDiscountServices
    {
        Task AddAsync(Discount discount);
        Task UpdateAsync(Discount discount);
        Task DeleteAsync(int id);
        Task<List<Discount>> GetAllAsync();
        Task<Discount> GetByIdAsync(int id);
        Task SaveChangesAsync();
        Task<decimal?> GetApplicableDiscountAsync(int productId, int categoryId);
        Task SyncDiscountsWithProductsAsync(IProductRepo productRepo);
    }
}
