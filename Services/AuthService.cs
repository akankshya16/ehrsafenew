using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserData;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
    public AuthService(IConfiguration config, ILogger<AuthService> logger)
    {
        _config = config;
        _logger = logger;

    }

    public string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32);

        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32);

        return hash.SequenceEqual(storedHashBytes);
    }


public string GenerateJwtToken(User user)
{
    try
    {
        if (user == null || string.IsNullOrEmpty(user.Email) || user.Id == 0)
        {
            _logger.LogWarning("Invalid user details provided for JWT token generation.");
            return string.Empty; // Return an empty string instead of throwing an error
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        string? keyString = _config["Jwt:Key"];
        string? issuer = _config["Jwt:Issuer"];
        string? audience = _config["Jwt:Audience"];

        if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            _logger.LogError("JWT configuration is missing.");
            return string.Empty;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating JWT token.");
        return string.Empty;
    }
}

}
