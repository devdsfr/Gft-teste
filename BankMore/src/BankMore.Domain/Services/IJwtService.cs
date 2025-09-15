using BankMore.Domain.Entities;

namespace BankMore.Domain.Services;

public interface IJwtService
{
    string GenerateToken(ContaCorrente conta);
    string? GetContaIdFromToken(string token);
}

