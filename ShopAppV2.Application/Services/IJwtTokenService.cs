namespace ShopApp.Application.Services;

public interface IJwtTokenService
{
    string GenerateToken(int userId, string username, string role);
}