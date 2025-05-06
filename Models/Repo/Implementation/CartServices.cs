public class CartServices : ICartServices
{
    private readonly IProductRepo _productRepo;
    private readonly IDiscountServices _discountServices;
    private readonly ApplicationDbContext _context;

    public CartServices(IProductRepo productRepo, IDiscountServices discountServices, ApplicationDbContext context)
    {
        _productRepo = productRepo;
        _discountServices = discountServices;
        _context = context;
    }

    public async Task<Result> AddToCartAsync(HttpContext httpContext, int productId, int quantity)
    {
        var product = await _productRepo.GetProductByIdAsync(productId);
        if (product == null)
        {
            return Result.Failure(new Error("Product.NotFound", "Product not found", StatusCodes.Status404NotFound));
        }

        if (quantity > product.Quantity)
        {
            return Result.Failure(new Error("Quantity.Invalid", $"Please enter a number less than available quantity: {product.Quantity}", StatusCodes.Status400BadRequest));
        }

        var discountPercentage = await _discountServices.GetApplicableDiscountAsync(productId, product.CategoryId);
        decimal discountedPrice = (decimal)product.Price;

        if (discountPercentage.HasValue && discountPercentage > 0)
        {
            discountedPrice *= (1 - (discountPercentage.Value / 100m));
        }

        var cart = httpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
            cartItem.OriginalPrice = cartItem.Price;
            cartItem.Price = (decimal)discountedPrice;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = productId,
                ProductName = product.Name,
                Quantity = quantity,
                OriginalPrice = (decimal)product.Price,
                Price = (decimal)discountedPrice
            });
        }

        await _productRepo.SaveChangesAsync();
        httpContext.Session.SetObjectAsJson("Cart", cart);
        return Result.Success();
    }

    public async Task<Result> RemoveFromCartAsync(HttpContext httpContext, int productId)
    {
        var cart = httpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
        if (cartItem != null)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);
            if (product != null)
            {
                product.Quantity += cartItem.Quantity;
                await _productRepo.SaveChangesAsync();
            }
            cart.Remove(cartItem);
        }

        httpContext.Session.SetObjectAsJson("Cart", cart);
        return Result.Success();
    }

    public async Task<Result> UpdateQuantityAsync(HttpContext httpContext, int productId, int newQuantity)
    {
        var cart = httpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);

        if (cartItem != null)
        {
            if (newQuantity <= 0)
            {
                cart.Remove(cartItem);
                var product = await _productRepo.GetProductByIdAsync(productId);
                if (product != null)
                {
                    product.Quantity += cartItem.Quantity;
                    await _productRepo.SaveChangesAsync();
                }
            }
            else
            {
                cartItem.Quantity = newQuantity;
            }
        }

        httpContext.Session.SetObjectAsJson("Cart", cart);
        return Result.Success();
    }

    public async Task<Result> ConfirmPurchaseAsync(HttpContext httpContext, ApplicationUser user)
    {
        var cart = httpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        if (!cart.Any())
        {
            return Result.Failure(new Error("Cart.Empty", "Your cart is empty.", StatusCodes.Status400BadRequest));
        }

        foreach (var item in cart)
        {
            var product = await _productRepo.GetProductByIdAsync(item.ProductId);
            if (product == null || product.Quantity < item.Quantity)
            {
                return Result.Failure(new Error("Stock.NotAvailable", $"Not enough stock for {item.ProductName}.", StatusCodes.Status400BadRequest));
            }
        }

        decimal totalPrice = 0;
        foreach (var item in cart)
        {
            var product = await _productRepo.GetProductByIdAsync(item.ProductId);
            var discount = await _discountServices.GetApplicableDiscountAsync(item.ProductId, product.CategoryId);
            var discountedPrice = item.OriginalPrice * (1 - discount / 100m);
            item.Price = (decimal)discountedPrice;
            totalPrice += (decimal)discountedPrice * item.Quantity;
        }

        var order = new Order
        {
            UserId = user.Id,
            OrderDate = DateTime.Now,
            TotalPrice = totalPrice,
            Status = "Pending",
            OrderItems = cart.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Price
            }).ToList()
        };

        _context.Orders.Add(order);

        foreach (var cartItem in cart)
        {
            var product = await _productRepo.GetProductByIdAsync(cartItem.ProductId);
            if (product != null)
            {
                product.Quantity = Math.Max(product.Quantity - cartItem.Quantity, 0);
                await _productRepo.UpdateAsync(product);
            }
        }

        await _productRepo.SaveChangesAsync();
        await _context.SaveChangesAsync();

        httpContext.Session.SetObjectAsJson("Cart", new List<CartItem>());

        return Result.Success();
    }
}
