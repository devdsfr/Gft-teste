using BankMore.Domain.Entities;
using BankMore.Domain.Repositories;
using BankMore.Infrastructure.Database;
using Dapper;
using System.Globalization;

namespace BankMore.Infrastructure.Repositories;

public class TransferenciaRepository : ITransferenciaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransferenciaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<string> CreateAsync(Transferencia transferencia)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO transferencia (idtransferencia, idcontacorrente_origem, idcontacorrente_destino, datamovimento, valor)
            VALUES (@IdTransferencia, @IdContaCorrenteOrigem, @IdContaCorrenteDestino, @DataMovimento, @Valor)";

        var dto = TransferenciaDto.FromEntity(transferencia);
        await connection.ExecuteAsync(sql, dto);
        
        return transferencia.IdTransferencia;
    }

    private class TransferenciaDto
    {
        public string IdTransferencia { get; set; } = string.Empty;
        public string IdContaCorrenteOrigem { get; set; } = string.Empty;
        public string IdContaCorrenteDestino { get; set; } = string.Empty;
        public string DataMovimento { get; set; } = string.Empty;
        public decimal Valor { get; set; }

        public static TransferenciaDto FromEntity(Transferencia transferencia)
        {
            return new TransferenciaDto
            {
                IdTransferencia = transferencia.IdTransferencia,
                IdContaCorrenteOrigem = transferencia.IdContaCorrenteOrigem,
                IdContaCorrenteDestino = transferencia.IdContaCorrenteDestino,
                DataMovimento = transferencia.DataMovimento.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                Valor = transferencia.Valor
            };
        }
    }
}

