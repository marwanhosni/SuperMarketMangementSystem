namespace SuperMarketMangementSystem.Models.Repo.Abstract;

public interface ICustomEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}
