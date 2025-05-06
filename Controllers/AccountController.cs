namespace SuperMarketMangementSystem.Controllers
{
    public class AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtProvider jwtProvider,
        IAuthServices authServices) : Controller
    {
        private readonly int ExpiresIn = 14;

        [HttpGet]
        public IActionResult Register() => View("Register");

        [HttpPost]
        public async Task<IActionResult> SaveRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await authServices.RegisterUserWithRoleAsync(model, "User");
                if (result.IsSuccess)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", result.Error.Description);
            }
            return View("Register", model);
        }

        [HttpGet]
        public IActionResult RoleAdminRegister() => View("RoleAdminRegister");

        [HttpPost]
        public async Task<IActionResult> SaveAdminRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await authServices.RegisterUserWithRoleAsync(model, "admin");
                if (result.IsSuccess)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", result.Error.Description);
            }
            return View("RoleAdminRegister", model);
        }

        [HttpGet]
        public IActionResult MangmentOfOrdersRegister() => View("MangmentOfOrdersRegister");

        [HttpPost]
        public async Task<IActionResult> SaveMangmentOfOrdersRegister(RegisterViewModel request)
        {
            if (ModelState.IsValid)
            {
                var result = await authServices.RegisterUserWithRoleAsync(request, "MangmentOfOrders");
                if (result.IsSuccess)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", result.Error.Description);
            }
            return View("MangmentOfOrdersRegister", request);
        }

        [HttpGet]
        public IActionResult Guest() => RedirectToAction("Index", "Home");

        [HttpGet]
        public IActionResult Login() => View("Login");

        [HttpPost]
        public async Task<IActionResult> SaveLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var authResult = await authServices.SaveLoginAsync(model);
                if (authResult.IsFailure)
                {
                    ModelState.AddModelError("", authResult.Error.Description);
                    return View("Login", model);
                }
                return RedirectToAction("RedirectByRole", "Home", new { Id = authResult.Value!.Id });
            }
            return View("Login", model);
        }

        [HttpGet]
        public IActionResult VerifyEmail() => View();

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authServices.SendPasswordResetTokenAsync(model.Email, Url, Request);
            if (result.IsFailure)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return View(model);
            }

            return View("EmailSent");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> SaveResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ResetPassword", model);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("Login");

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.NewPassword);
            if (verificationResult == PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "You should enter a different password.");
                return View("ResetPassword", model);
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
                return RedirectToAction("Login");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View("ResetPassword", model);
        }

        [HttpGet]
        public async Task<IActionResult> ShowUserToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Login");

            var (token, expiresIn) = jwtProvider.GenerateToken(user);
            var response = new AuthResponse(user.Id, user.Email!, token, expiresIn);
            return View("ShowToken", response);
        }

        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return View("Login");
        }

        public IActionResult Index() => View();
    }
}
