namespace SuperMarketMangementSystem.Models.Repo.Abstract
{
    public interface IProductRepo
    {
        Task AddAsync(Products products);
        Task UpdateAsync(Products products);
        Task<List<Products>> GetAllProductsAsync();
        Task<Products> GetProductByIdAsync(int id);
        Task DeleteAsync(int Id);
        Task SaveChangesAsync();
    }
}
