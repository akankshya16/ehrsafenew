using UserData;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string storedHash);
    string GenerateJwtToken(User user);
}
