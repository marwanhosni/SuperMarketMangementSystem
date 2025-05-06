
public class CartController(
    IProductRepo productRepo,
    ICategoryRepo categoryRepo,
    ICartServices cartServices,
     IDiscountServices discountServices,
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly IProductRepo _productRepo = productRepo;
    private readonly ICategoryRepo _categoryRepo = categoryRepo;
    private readonly ICartServices _cartServices = cartServices;
    private readonly IDiscountServices _discountServices = discountServices;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IActionResult> ViewCart()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        ViewBag.ProductRepo = _productRepo; // استخدام _productRepo بدلاً من ViewBag
        ViewBag.DiscountServices = _discountServices;
        return View("ViewCart", cart);
    }


    public async Task<IActionResult> AddToCart(int productId, int quantity)
    {
        var result = await _cartServices.AddToCartAsync(HttpContext, productId, quantity);

        if (!result.IsSuccess)
        {
            TempData["CartError"] = result.Error.Description;
            return RedirectToAction("ProductDetails", "Product", new { id = productId });
        }

        return RedirectToAction("ViewCart");
    }

    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var result = await _cartServices.RemoveFromCartAsync(HttpContext, productId);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Description;
        }
        return RedirectToAction("ViewCart");
    }

    public async Task<IActionResult> UpdateQuantity(int productId, int newQuantity)
    {
        var result = await _cartServices.UpdateQuantityAsync(HttpContext, productId, newQuantity);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Description;
        }
        return RedirectToAction("ViewCart");
    }


    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> ConfirmPurchase()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("Login", "Account");
        }

        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        var result = await _cartServices.ConfirmPurchaseAsync(HttpContext, user);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error.Description;
            return RedirectToAction("ViewCart");
        }

        TempData["SuccessMessage"] = "Order placed successfully!";
        return View("ConfirmPurchase", cart);
    }
}
