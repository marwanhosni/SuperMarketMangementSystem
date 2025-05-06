namespace SuperMarketMangementSystem.Controllers
{
    public class DiscountController : Controller
    {
        private readonly IDiscountServices discountServices;
        private readonly IProductRepo productRepo;
        private readonly ICategoryRepo categoryRepo;

        public DiscountController(IDiscountServices discountServices, IProductRepo productRepo, ICategoryRepo categoryRepo)
        {
            this.discountServices = discountServices;
            this.productRepo = productRepo;
            this.categoryRepo = categoryRepo;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AllDiscounts(string sortBy = "EndDate")
        {
            var discounts = await discountServices.GetAllAsync();

            discounts = sortBy switch
            {
                "StartDate" => discounts.OrderBy(d => d.StartDate).ToList(),
                "DiscountPercentage" => discounts.OrderByDescending(d => d.DiscountPercentage).ToList(),
                _ => discounts.OrderBy(d => d.EndDate).ToList() 
            };

            ViewBag.CurrentSort = sortBy;
            return View("AllDiscounts", discounts);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> AddDiscount()
        {
            ViewBag.Products = await productRepo.GetAllProductsAsync();
            ViewBag.Categories = await categoryRepo.GetAllCategoriesAsync();
            return View("AddDiscount");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> SaveAddDiscount(Discount discount)
        {
            if (string.IsNullOrWhiteSpace(discount.Name))
            {
                ModelState.AddModelError("Name", "Discount name is required");
            }
            if (discount.DiscountPercentage <= 0)
            {
                ModelState.AddModelError("DiscountPercentage", "Discount percentage must be greater than 0");
            }
            if (discount.StartDate == default(DateTime))
            {
                ModelState.AddModelError("StartDate", "Start date is required");
            }
            if (discount.EndDate == default(DateTime))
            {
                ModelState.AddModelError("EndDate", "End date is required");
            }
            if (discount.EndDate < discount.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after the start date");
            }

            if (ModelState.IsValid)
            {
                await discountServices.AddAsync(discount);
                await discountServices.SaveChangesAsync();

                return RedirectToAction("AllDiscounts");
            }

            ViewBag.Products = await productRepo.GetAllProductsAsync();
            ViewBag.Categories = await categoryRepo.GetAllCategoriesAsync();
            return View("AddDiscount", discount);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> EditDiscount(int id)
        {
            var discount = await discountServices.GetByIdAsync(id);
            if (discount == null)
                return RedirectToAction("AllDiscounts");

            ViewBag.Products = await productRepo.GetAllProductsAsync();
            ViewBag.Categories = await categoryRepo.GetAllCategoriesAsync();
            return View("EditDiscount", discount);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> SaveEditDiscount(Discount discount)
        {
            if (string.IsNullOrWhiteSpace(discount.Name))
            {
                ModelState.AddModelError("Name", "Discount name is required");
            }
            if (discount.DiscountPercentage <= 0)
            {
                ModelState.AddModelError("DiscountPercentage", "Discount percentage must be greater than 0");
            }
            if (discount.StartDate == default(DateTime))
            {
                ModelState.AddModelError("StartDate", "Start date is required");
            }
            if (discount.EndDate == default(DateTime))
            {
                ModelState.AddModelError("EndDate", "End date is required");
            }
            if (discount.EndDate < discount.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after the start date");
            }

            if (ModelState.IsValid)
            {
                var originalDiscount = await discountServices.GetByIdAsync(discount.Id);
                if (originalDiscount != null)
                {
                    if (originalDiscount.ProductId.HasValue && originalDiscount.ProductId != discount.ProductId)
                    {
                        var oldProduct = await productRepo.GetProductByIdAsync(originalDiscount.ProductId.Value);
                        if (oldProduct != null)
                        {
                            oldProduct.DiscountId = null;
                            await productRepo.UpdateAsync(oldProduct);
                        }
                    }

                    if (discount.ProductId.HasValue)
                    {
                        var newProduct = await productRepo.GetProductByIdAsync(discount.ProductId.Value);
                        if (newProduct != null)
                        {
                            newProduct.DiscountId = discount.Id;
                            await productRepo.UpdateAsync(newProduct);
                        }
                    }
                }

                await discountServices.UpdateAsync(discount);
                await discountServices.SaveChangesAsync();
                return RedirectToAction("AllDiscounts");
            }

            ViewBag.Products = await productRepo.GetAllProductsAsync();
            ViewBag.Categories = await categoryRepo.GetAllCategoriesAsync();
            return View("EditDiscount", discount);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var discount = await discountServices.GetByIdAsync(id);
            if (discount == null)
                return RedirectToAction("AllDiscounts");

            return View("ConfirmDelete", discount);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var discount = await discountServices.GetByIdAsync(id);
            if (discount != null)
            {
                await discountServices.DeleteAsync(id);
                return RedirectToAction("AllDiscounts");
            }

            return RedirectToAction("AllDiscounts");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await productRepo.GetAllProductsAsync();

            var filteredProducts = products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new { p.Id, p.Name })
                .ToList();

            return Json(filteredProducts);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> DiscountedProducts(string sortBy = "EndDate")
        {
            var today = DateTime.Today;

            var allProducts = await productRepo.GetAllProductsAsync();

            var discountedProducts = allProducts
                .Where(p => p.Discount != null &&
                            p.Discount.StartDate <= today &&
                            p.Discount.EndDate >= today)
                .ToList();

            discountedProducts = sortBy switch
            {
                "StartDate" => discountedProducts.OrderBy(p => p.Discount.StartDate).ToList(),
                "DiscountPercentage" => discountedProducts.OrderByDescending(p => p.Discount.DiscountPercentage).ToList(),
                _ => discountedProducts.OrderBy(p => p.Discount.EndDate).ToList() 
            };

            ViewBag.CurrentSort = sortBy;
            return View("DiscountedProducts", discountedProducts);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> SyncDiscountsWithProducts()
        {
            await discountServices.SyncDiscountsWithProductsAsync(productRepo);
            TempData["SuccessMessage"] = "Discounts have been synced with products successfully!";
            return RedirectToAction("AllDiscounts");
        }
    }
}
