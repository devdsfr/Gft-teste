using BankMore.Domain.Entities;

namespace BankMore.Domain.Repositories;

public interface IMovimentoRepository
{
    Task<string> CreateAsync(Movimento movimento);
    Task<decimal> GetSaldoByContaAsync(string idContaCorrente);
}

