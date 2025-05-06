namespace SuperMarketMangementSystem.Models.Repo.Implementation
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly ICustomEmailSender _emailSender;

        public AuthServices(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtProvider jwtProvider,
            ICustomEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtProvider = jwtProvider;
            _emailSender = emailSender;
        }

        public async Task<Result> RegisterUserWithRoleAsync(RegisterViewModel request, string role)
        {
            // If Name Or Email Is Already Existing
            var usernameCheckResult = await CheckUsernameAvailabilityAsync(request.Name);
            if (usernameCheckResult.IsFailure)
                return usernameCheckResult;

            var emailCheckResult = await CheckEmailAvailabilityAsync(request.Email);
            if (emailCheckResult.IsFailure)
                return emailCheckResult;

            var user = new ApplicationUser
            {
                UserName = request.Name,
                Email = request.Email,
                PasswordHash = request.password,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                AnotherPhoneNumber = request.AnotherPhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, request.password);
            if (!result.Succeeded)
            {
                var error1 = result.Errors.First();
                return Result.Failure(new Error(error1.Code, error1.Description, StatusCodes.Status400BadRequest));
            }

            var assignToRole = await _userManager.AddToRoleAsync(user, role);
            if (!assignToRole.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            await _signInManager.SignInAsync(user, false);
            return Result.Success();
        }

        public async Task<Result<AuthResponse?>> SaveLoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Result.Failure<AuthResponse?>(new Error("InvalidCredentials", "The Name Or Password Or Email Aren't Correct", StatusCodes.Status400BadRequest));

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.password);
            if (!isPasswordCorrect)
                return Result.Failure<AuthResponse?>(new Error("InvalidCredentials", "The Name Or Password Aren't Correct", StatusCodes.Status400BadRequest));

            await _signInManager.SignInAsync(user, model.RemeberMe);

            if (string.IsNullOrEmpty(user.Id))
                return Result.Failure<AuthResponse?>(new Error(nameof(user.Id), "User Id is null or empty", StatusCodes.Status400BadRequest));

            if (string.IsNullOrEmpty(user.Email))
                return Result.Failure<AuthResponse?>(new Error(nameof(user.Email), "User Email is null or empty", StatusCodes.Status400BadRequest));

            var (token, expiresIn) = _jwtProvider.GenerateToken(user);

            return Result.Successs<AuthResponse?>(new AuthResponse(user.Id, user.Email, token, expiresIn));
        }


        public async Task<Result> SendPasswordResetTokenAsync(string email, IUrlHelper urlHelper, HttpRequest request)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure(new Error("NotFound", "User not found", 404));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = urlHelper.Action("ResetPassword", "Account", new
            {
                Email = email,
                Token = token
            }, request.Scheme);

            await _emailSender.SendEmailAsync(email, "Reset your password", $"Click this link to reset your password: <a href='{resetLink}'>Reset Password</a>");

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Result.Failure(new Error("InvalidCredentials", "The Email Isn't Correct", StatusCodes.Status400BadRequest));

            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                return Result.Failure(new Error("InvalidCredentials", "Something Wrong Try Again", StatusCodes.Status400BadRequest));
            }

            result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            return Result.Success();
        }

        public async Task<Result> VerifyEmailAsync(VerifyEmailViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Result.Failure(new Error("InvalidCredentials", "The Email Isn't Correct", StatusCodes.Status400BadRequest));

            return Result.Success();
        }
        private async Task<Result> CheckUsernameAvailabilityAsync(string username)
        {
            var existingUserWithName = await _userManager.FindByNameAsync(username);
            if (existingUserWithName != null)
            {
                return Result.Failure(new Error("Username.AlreadyExists", "The username is already taken. Please choose a different one.", StatusCodes.Status400BadRequest));
            }
            return Result.Success();
        }
        private async Task<Result> CheckEmailAvailabilityAsync(string Email)
        {
            var existingUserWithEmail = await _userManager.FindByEmailAsync(Email);
            if (existingUserWithEmail != null)
            {
                return Result.Failure(new Error("Email.AlreadyExists", "The Email is already Have An Account. Please Try A New Email.", StatusCodes.Status400BadRequest));
            }
            return Result.Success();
        }
    }
}
