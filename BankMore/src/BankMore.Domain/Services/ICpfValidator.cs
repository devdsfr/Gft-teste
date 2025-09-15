namespace BankMore.Domain.Services;

public interface ICpfValidator
{
    bool IsValid(string cpf);
}

