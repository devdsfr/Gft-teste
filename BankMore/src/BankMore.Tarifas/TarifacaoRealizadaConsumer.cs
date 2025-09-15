using BankMore.Domain.Entities;
using BankMore.Domain.Events;
using BankMore.Infrastructure.Database;
using BankMore.Infrastructure.Repositories;
using KafkaFlow;
using System.Text.Json;

namespace BankMore.Tarifas;

public class TarifacaoRealizadaConsumer : IMessageHandler<string>
{
    private readonly ILogger<TarifacaoRealizadaConsumer> _logger;
    private readonly IConfiguration _configuration;

    public TarifacaoRealizadaConsumer(
        ILogger<TarifacaoRealizadaConsumer> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task Handle(IMessageContext context, string message)
    {
        try
        {
            _logger.LogInformation("Processando tarifação realizada: {message}", message);

            var tarifacao = JsonSerializer.Deserialize<TarifacaoRealizadaEvent>(message);
            if (tarifacao == null)
            {
                _logger.LogWarning("Mensagem inválida recebida: {message}", message);
                return;
            }

            var connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=/app/data/bankmore.db";
            
            var connectionFactory = new SqliteConnectionFactory(connectionString);
            var movimentoRepository = new MovimentoRepository(connectionFactory);

            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = tarifacao.IdContaCorrente,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'D',
                Valor = tarifacao.ValorTarifado
            };

            await movimentoRepository.CreateAsync(movimento);

            _logger.LogInformation("Débito de tarifa processado com sucesso para conta {conta}, valor {valor}", 
                tarifacao.IdContaCorrente, tarifacao.ValorTarifado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar tarifação realizada: {message}", message);
            throw;
        }
    }
}

