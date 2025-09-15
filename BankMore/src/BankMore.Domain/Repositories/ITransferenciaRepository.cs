using BankMore.Domain.Entities;

namespace BankMore.Domain.Repositories;

public interface ITransferenciaRepository
{
    Task<string> CreateAsync(Transferencia transferencia);
}

