using BankMore.Domain.Services;
using System.Security.Cryptography;
using System.Text;

namespace BankMore.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public (string hash, string salt) HashPassword(string password)
    {
        var saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        var salt = Convert.ToBase64String(saltBytes);

        var hash = HashPasswordWithSalt(password, salt);

        return (hash, salt);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var computedHash = HashPasswordWithSalt(password, salt);
        return computedHash == hash;
    }

    private static string HashPasswordWithSalt(string password, string salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password + salt);
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(passwordBytes);
        return Convert.ToBase64String(hashBytes);
    }
}

