namespace SuperMarketMangementSystem.Models.Repo;

public class UserError
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid Email Or Paswword", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid  Jwt Token", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
     new("User.InvalidRefreshToken", "Invalid Refresh Token", StatusCodes.Status401Unauthorized);

    public static readonly Error DupilcatedEmail =
    new("User.DupilcatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);

    public static readonly Error InvalidCode =
    new("User.InvalidCode", "Code is in valid", StatusCodes.Status401Unauthorized);

    public static readonly Error EmailIsAlreadyConfirmed =
    new("User.EmailIsAlreadyConfirmed", "Email is already have been confirmed", StatusCodes.Status400BadRequest);

    public static readonly Error EmailNotConfirmed =
    new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

}
