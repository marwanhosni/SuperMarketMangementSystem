namespace SuperMarketMangementSystem.Models.Repo.Abstract;

public interface IAuthServices
{
    Task<Result<AuthResponse?>> SaveLoginAsync(LoginViewModel request);
    Task<Result> RegisterUserWithRoleAsync(RegisterViewModel request, string role);
    Task<Result> VerifyEmailAsync(VerifyEmailViewModel model);
    Task<Result> ChangePasswordAsync(ChangePasswordViewModel model);
    Task<Result> SendPasswordResetTokenAsync(string email, IUrlHelper urlHelper, HttpRequest request);
}
