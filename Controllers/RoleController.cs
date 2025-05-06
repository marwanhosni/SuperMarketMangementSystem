namespace SuperMarketMangementSystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public IActionResult AddRole()
        {
            return View("AddRole");
        }
        public async  Task<IActionResult> SaveRole(RoleViewModel RoleFromReq) 
        {
            if (ModelState.IsValid) 
            {

                var role = new IdentityRole
                {
                    Name = RoleFromReq.RoleName
                };


                IdentityResult result = await roleManager.CreateAsync(role);
                if (result.Succeeded) 
                {
                    ViewBag.success = true;
                    return View("Addrole");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("",item.Description);

                }

            }
            return View("AddRole", RoleFromReq);
        } 

        public IActionResult Index()
        {
            return View();
        }
    }
}
