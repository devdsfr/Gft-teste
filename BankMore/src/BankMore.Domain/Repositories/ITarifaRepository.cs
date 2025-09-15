using BankMore.Domain.Entities;

namespace BankMore.Domain.Repositories;

public interface ITarifaRepository
{
    Task<string> CreateAsync(Tarifa tarifa);
}

