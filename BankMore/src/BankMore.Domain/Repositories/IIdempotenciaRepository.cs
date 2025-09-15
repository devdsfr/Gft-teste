using BankMore.Domain.Entities;

namespace BankMore.Domain.Repositories;

public interface IIdempotenciaRepository
{
    Task<Idempotencia?> GetByChaveAsync(string chave);
    Task CreateAsync(Idempotencia idempotencia);
}

