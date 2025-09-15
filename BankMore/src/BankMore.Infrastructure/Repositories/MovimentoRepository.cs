using BankMore.Domain.Entities;
using BankMore.Domain.Repositories;
using BankMore.Infrastructure.Database;
using Dapper;
using System.Globalization;

namespace BankMore.Infrastructure.Repositories;

public class MovimentoRepository : IMovimentoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MovimentoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<string> CreateAsync(Movimento movimento)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
            VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

        var dto = MovimentoDto.FromEntity(movimento);
        await connection.ExecuteAsync(sql, dto);
        
        return movimento.IdMovimento;
    }

    public async Task<decimal> GetSaldoByContaAsync(string idContaCorrente)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT 
                COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) -
                COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) as Saldo
            FROM movimento 
            WHERE idcontacorrente = @IdContaCorrente";

        return await connection.QuerySingleAsync<decimal>(sql, new { IdContaCorrente = idContaCorrente });
    }

    private class MovimentoDto
    {
        public string IdMovimento { get; set; } = string.Empty;
        public string IdContaCorrente { get; set; } = string.Empty;
        public string DataMovimento { get; set; } = string.Empty;
        public string TipoMovimento { get; set; } = string.Empty;
        public decimal Valor { get; set; }

        public static MovimentoDto FromEntity(Movimento movimento)
        {
            return new MovimentoDto
            {
                IdMovimento = movimento.IdMovimento,
                IdContaCorrente = movimento.IdContaCorrente,
                DataMovimento = movimento.DataMovimento.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                TipoMovimento = movimento.TipoMovimento.ToString(),
                Valor = movimento.Valor
            };
        }
    }
}

