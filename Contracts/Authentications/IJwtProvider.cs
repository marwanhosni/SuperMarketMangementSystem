using SuperMarketMangementSystem.Models.Entities;
using System.Data;

namespace SuperMarketMangementSystem.Contracts.Authentications;

public interface IJwtProvider
{
    (string Token, int ExpiresIn) GenerateToken(ApplicationUser user);

    string? ValidateToken(string token);
}
