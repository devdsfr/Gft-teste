using BankMore.Domain.Entities;

namespace BankMore.Domain.Repositories;

public interface IContaCorrenteRepository
{
    Task<ContaCorrente?> GetByIdAsync(string id);
    Task<ContaCorrente?> GetByCpfAsync(string cpf);
    Task<ContaCorrente?> GetByNumeroAsync(int numero);
    Task<string> CreateAsync(ContaCorrente conta);
    Task UpdateAsync(ContaCorrente conta);
    Task<int> GetNextNumeroContaAsync();
    Task<bool> ExistsByCpfAsync(string cpf);
}

