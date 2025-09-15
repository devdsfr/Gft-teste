using BankMore.Domain.Entities;
using BankMore.Domain.Repositories;
using BankMore.Infrastructure.Database;
using Dapper;
using System.Globalization;

namespace BankMore.Infrastructure.Repositories;

public class ContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ContaCorrenteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ContaCorrente?> GetByIdAsync(string id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT idcontacorrente as IdContaCorrente, 
                   numero as Numero, 
                   cpf as Cpf, 
                   nome as Nome, 
                   ativo as Ativo, 
                   senha as Senha, 
                   salt as Salt, 
                   datacriacao as DataCriacao
            FROM contacorrente 
            WHERE idcontacorrente = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<ContaCorrenteDto>(sql, new { Id = id });
        return result?.ToEntity();
    }

    public async Task<ContaCorrente?> GetByCpfAsync(string cpf)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT idcontacorrente as IdContaCorrente, 
                   numero as Numero, 
                   cpf as Cpf, 
                   nome as Nome, 
                   ativo as Ativo, 
                   senha as Senha, 
                   salt as Salt, 
                   datacriacao as DataCriacao
            FROM contacorrente 
            WHERE cpf = @Cpf";

        var result = await connection.QueryFirstOrDefaultAsync<ContaCorrenteDto>(sql, new { Cpf = cpf });
        return result?.ToEntity();
    }

    public async Task<ContaCorrente?> GetByNumeroAsync(int numero)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT idcontacorrente as IdContaCorrente, 
                   numero as Numero, 
                   cpf as Cpf, 
                   nome as Nome, 
                   ativo as Ativo, 
                   senha as Senha, 
                   salt as Salt, 
                   datacriacao as DataCriacao
            FROM contacorrente 
            WHERE numero = @Numero";

        var result = await connection.QueryFirstOrDefaultAsync<ContaCorrenteDto>(sql, new { Numero = numero });
        return result?.ToEntity();
    }

    public async Task<string> CreateAsync(ContaCorrente conta)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO contacorrente (idcontacorrente, numero, cpf, nome, ativo, senha, salt, datacriacao)
            VALUES (@IdContaCorrente, @Numero, @Cpf, @Nome, @Ativo, @Senha, @Salt, @DataCriacao)";

        var dto = ContaCorrenteDto.FromEntity(conta);
        await connection.ExecuteAsync(sql, dto);
        
        return conta.IdContaCorrente;
    }

    public async Task UpdateAsync(ContaCorrente conta)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            UPDATE contacorrente 
            SET numero = @Numero, 
                cpf = @Cpf, 
                nome = @Nome, 
                ativo = @Ativo, 
                senha = @Senha, 
                salt = @Salt, 
                datacriacao = @DataCriacao
            WHERE idcontacorrente = @IdContaCorrente";

        var dto = ContaCorrenteDto.FromEntity(conta);
        await connection.ExecuteAsync(sql, dto);
    }

    public async Task<int> GetNextNumeroContaAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT COALESCE(MAX(numero), 0) + 1 FROM contacorrente";
        return await connection.QuerySingleAsync<int>(sql);
    }

    public async Task<bool> ExistsByCpfAsync(string cpf)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT COUNT(1) FROM contacorrente WHERE cpf = @Cpf";
        var count = await connection.QuerySingleAsync<int>(sql, new { Cpf = cpf });
        return count > 0;
    }

    private class ContaCorrenteDto
    {
        public string IdContaCorrente { get; set; } = string.Empty;
        public int Numero { get; set; }
        public string Cpf { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int Ativo { get; set; }
        public string Senha { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string DataCriacao { get; set; } = string.Empty;

        public ContaCorrente ToEntity()
        {
            return new ContaCorrente
            {
                IdContaCorrente = IdContaCorrente,
                Numero = Numero,
                Cpf = Cpf,
                Nome = Nome,
                Ativo = Ativo == 1,
                Senha = Senha,
                Salt = Salt,
                DataCriacao = DateTime.ParseExact(DataCriacao, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
            };
        }

        public static ContaCorrenteDto FromEntity(ContaCorrente conta)
        {
            return new ContaCorrenteDto
            {
                IdContaCorrente = conta.IdContaCorrente,
                Numero = conta.Numero,
                Cpf = conta.Cpf,
                Nome = conta.Nome,
                Ativo = conta.Ativo ? 1 : 0,
                Senha = conta.Senha,
                Salt = conta.Salt,
                DataCriacao = conta.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
            };
        }
    }
}

