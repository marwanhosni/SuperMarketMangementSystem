namespace SuperMarketMangementSystem.Models.Repo.Abstract
{
    public interface IOrdersService
    {
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    }
}
