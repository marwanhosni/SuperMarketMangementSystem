namespace SuperMarketMangementSystem.Controllers
{
    public class CategoryController(
        ICategoryRepo categoryRepo
    ) : Controller
    {
        public async Task<IActionResult> AllCategory()
        {
            var categories = await categoryRepo.GetAllCategoriesAsync();
            return View("AllCategory", categories);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int Id)
        {
            var category = await categoryRepo.GetByIdAsync(Id);
            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("EditCategory", category);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEditCategory(Category categoryFromReq)
        {
            if (ModelState.IsValid)
            {
                var categoryDb = await categoryRepo.GetByIdAsync(categoryFromReq.Id);
                if (categoryDb == null)
                {
                    return Content("Category not found.");
                }
                categoryDb.Name = categoryFromReq.Name;
                categoryDb.Description = categoryFromReq.Description;
                await categoryRepo.UpdateCategoryAsync(categoryDb);
                return RedirectToAction("AllCategory");
            }
            return View("EditCategory", categoryFromReq);
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                return Content("Category not found.");
            }
            await categoryRepo.DeleteCategoryAsync(id);
            return RedirectToAction("AllCategory");
        }
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var category = await categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(); 
            }
            return View(category); 
        }
        [HttpGet]
        public IActionResult NewCategory()
        {
            return View(); 
        }

        public async Task<IActionResult> SaveNewCategory(Category categoryFromReq)
        {
            if (ModelState.IsValid)
            {
                await categoryRepo.AddCategoryAsync(categoryFromReq);
                return RedirectToAction("AllCategory");
            }
            return View("AllCategory");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
