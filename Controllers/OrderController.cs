namespace SuperMarketMangementSystem.Controllers;

[Authorize]
public class OrdersController(IOrdersService ordersService) : Controller
{
    private readonly IOrdersService _ordersService = ordersService;

    [Authorize(Roles = "User")]
    public async Task<IActionResult> MyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _ordersService.GetUserOrdersAsync(userId);
        return View("MyOrders", orders);
    }

    [Authorize(Roles = "admin,MangmentOfOrders")]
    public async Task<IActionResult> AllOrders()
    {
        var orders = await _ordersService.GetAllOrdersAsync();
        return View("AllOrders", orders);
    }

    [Authorize(Roles = "admin,MangmentOfOrders")]
    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
    {
        var success = await _ordersService.UpdateOrderStatusAsync(orderId, status);

        if (!success)
        {
            TempData["ErrorMessage"] = "An error occurred while updating the order status or the provided value is invalid.";
            return RedirectToAction("AllOrders");

        }

        TempData["SuccessMessage"] = $"Order #{orderId} status has been updated to {status}.";
        return RedirectToAction("AllOrders");
    }
}
