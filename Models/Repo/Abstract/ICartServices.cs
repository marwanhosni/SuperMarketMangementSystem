using SuperMarketMangementSystem.Models.Repo;

public interface ICartServices
{
    Task<Result> AddToCartAsync(HttpContext httpContext, int productId, int quantity);
    Task<Result> RemoveFromCartAsync(HttpContext httpContext, int productId);
    Task<Result> UpdateQuantityAsync(HttpContext httpContext, int productId, int newQuantity);
    Task<Result> ConfirmPurchaseAsync(HttpContext httpContext, ApplicationUser user);
}
