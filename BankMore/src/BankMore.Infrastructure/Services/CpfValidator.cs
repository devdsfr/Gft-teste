using BankMore.Domain.Services;

namespace BankMore.Infrastructure.Services;

public class CpfValidator : ICpfValidator
{
    public bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");

        if (cpf.Length != 11)
            return false;

        if (cpf.All(c => c == cpf[0]))
            return false;

        if (!cpf.All(char.IsDigit))
            return false;

        var soma = 0;
        for (int i = 0; i < 9; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (10 - i);
        }

        var resto = soma % 11;
        var digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
            return false;

        soma = 0;
        for (int i = 0; i < 10; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (11 - i);
        }

        resto = soma % 11;
        var digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cpf[10].ToString()) == digitoVerificador2;
    }
}

