using BankMore.Domain.Entities;
using BankMore.Domain.Repositories;
using BankMore.Infrastructure.Database;
using Dapper;
using System.Globalization;

namespace BankMore.Infrastructure.Repositories;

public class TarifaRepository : ITarifaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TarifaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<string> CreateAsync(Tarifa tarifa)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO tarifa (idtarifa, idcontacorrente, datamovimento, valor)
            VALUES (@IdTarifa, @IdContaCorrente, @DataMovimento, @Valor)";

        var dto = TarifaDto.FromEntity(tarifa);
        await connection.ExecuteAsync(sql, dto);
        
        return tarifa.IdTarifa;
    }

    private class TarifaDto
    {
        public string IdTarifa { get; set; } = string.Empty;
        public string IdContaCorrente { get; set; } = string.Empty;
        public string DataMovimento { get; set; } = string.Empty;
        public decimal Valor { get; set; }

        public static TarifaDto FromEntity(Tarifa tarifa)
        {
            return new TarifaDto
            {
                IdTarifa = tarifa.IdTarifa,
                IdContaCorrente = tarifa.IdContaCorrente,
                DataMovimento = tarifa.DataMovimento.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                Valor = tarifa.Valor
            };
        }
    }
}

