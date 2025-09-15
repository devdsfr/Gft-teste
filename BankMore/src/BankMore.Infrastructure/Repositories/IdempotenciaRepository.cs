using BankMore.Domain.Entities;
using BankMore.Domain.Repositories;
using BankMore.Infrastructure.Database;
using Dapper;

namespace BankMore.Infrastructure.Repositories;

public class IdempotenciaRepository : IIdempotenciaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public IdempotenciaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Idempotencia?> GetByChaveAsync(string chave)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT chave_idempotencia as ChaveIdempotencia, 
                   requisicao as Requisicao, 
                   resultado as Resultado
            FROM idempotencia 
            WHERE chave_idempotencia = @Chave";

        return await connection.QueryFirstOrDefaultAsync<Idempotencia>(sql, new { Chave = chave });
    }

    public async Task CreateAsync(Idempotencia idempotencia)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
            VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";

        await connection.ExecuteAsync(sql, idempotencia);
    }
}

