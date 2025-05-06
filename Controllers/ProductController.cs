namespace SuperMarketMangementSystem.Controllers
{
    public class ProductController(
        IProductRepo productRepo,
        ICategoryRepo categoryRepo
    ) : Controller
    {
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await productRepo.GetAllProductsAsync();
            return View("GetAllProduct", products);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await productRepo.GetProductByIdAsync(id);
            if (product == null)
                return RedirectToAction("GetAllProduct");

            var categoriesList = await categoryRepo.GetAllCategoriesAsync();
            var productCategoryVM = new ProductCategoryViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Quantity = product.Quantity,
                Price = product.Price,
                OldImage = product.Image,
                CategryId = product.CategoryId,
                categoriesList = categoriesList
            };

            return View("Edit", productCategoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEdit(ProductCategoryViewModel productCategoryFromRequest)
        {
            if (ModelState.IsValid)
            {
                var productFromDb = await productRepo.GetProductByIdAsync(productCategoryFromRequest.Id);
                if (productFromDb != null)
                {
                    productFromDb.Name = productCategoryFromRequest.Name;
                    productFromDb.Price = productCategoryFromRequest.Price;
                    productFromDb.Quantity = productCategoryFromRequest.Quantity;
                    productFromDb.CategoryId = productCategoryFromRequest.CategryId;

                    if (productCategoryFromRequest.Image != null && productCategoryFromRequest.Image.Length > 0)
                    {
                        string uniqueFileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_" + Path.GetFileName(productCategoryFromRequest.Image.FileName);
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await productCategoryFromRequest.Image.CopyToAsync(fileStream);
                        }
                        productFromDb.Image = uniqueFileName;
                    }
                    else
                    {
                        productFromDb.Image = productCategoryFromRequest.OldImage;
                    }

                    await productRepo.UpdateAsync(productFromDb);
                    await productRepo.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction("GetAllProduct");
                }
                return View("Edit", productCategoryFromRequest);
            }

            productCategoryFromRequest.categoriesList = await categoryRepo.GetAllCategoriesAsync();
            return View("Edit", productCategoryFromRequest);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var products = await productRepo.GetProductByIdAsync(id);
            if (products == null)
            {
                return Content("Product Isn't Correct");
            }
            await productRepo.DeleteAsync(id);
            await productRepo.SaveChangesAsync();
            return RedirectToAction("GetAllProduct");
        }
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            Products products = await productRepo.GetProductByIdAsync(id);
            if (products == null)
            {
                return Content("Product Isn Not Coorect");
            }
            return View("ConfirmDelete", products);
        }

        [HttpGet]
        public async Task<IActionResult> AddNewProduct()
        {
            var categoryList = await categoryRepo.GetAllCategoriesAsync();
            var productCategoryViewModel = new ProductCategoryViewModel
            {
                categoriesList = categoryList
            };
            return View("AddNewProduct", productCategoryViewModel);
        }

        public async Task<IActionResult> SaveAddNewProduct(ProductCategoryViewModel productCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                Products newProductDb = new Products
                {
                    Name = productCategoryViewModel.Name,
                    Price = productCategoryViewModel.Price,
                    Quantity = productCategoryViewModel.Quantity,
                    CategoryId = productCategoryViewModel.CategryId
                };

                if (productCategoryViewModel.Image != null && productCategoryViewModel.Image.Length > 0)
                {
                    string uniqueFileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_" + Path.GetFileName(productCategoryViewModel.Image.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productCategoryViewModel.Image.CopyToAsync(fileStream);
                    }
                    newProductDb.Image = uniqueFileName;
                }

                await productRepo.AddAsync(newProductDb);
                await productRepo.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product Added successfully!";
                return RedirectToAction("GetAllProduct");
            }

            productCategoryViewModel.categoriesList =await categoryRepo.GetAllCategoriesAsync();
            return View("AddNewProduct", productCategoryViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await productRepo.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var category = await categoryRepo.GetByIdAsync(product.CategoryId);
            if (category == null)
                return NotFound();

            var productDetailsVM = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity,
                Description = category.Description,
                Image = product.Image
            };

            return View("Details", productDetailsVM);
        }

        public IActionResult ViewImage(string imageName)
        {
            ViewBag.ImageName = imageName;
            return View();
        }
    }
}
