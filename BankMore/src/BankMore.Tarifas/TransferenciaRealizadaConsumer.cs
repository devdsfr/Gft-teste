using BankMore.Domain.Entities;
using BankMore.Domain.Events;
using BankMore.Infrastructure.Database;
using BankMore.Infrastructure.Repositories;
using KafkaFlow;
using KafkaFlow.Producers;
using System.Text.Json;

namespace BankMore.Tarifas;

public class TransferenciaRealizadaConsumer : IMessageHandler<string>
{
    private readonly ILogger<TransferenciaRealizadaConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMessageProducer _producer;

    public TransferenciaRealizadaConsumer(
        ILogger<TransferenciaRealizadaConsumer> logger,
        IConfiguration configuration,
        IMessageProducer producer)
    {
        _logger = logger;
        _configuration = configuration;
        _producer = producer;
    }

    public async Task Handle(IMessageContext context, string message)
    {
        try
        {
            _logger.LogInformation("Processando transferência realizada: {message}", message);

            var transferencia = JsonSerializer.Deserialize<TransferenciaRealizadaEvent>(message);
            if (transferencia == null)
            {
                _logger.LogWarning("Mensagem inválida recebida: {message}", message);
                return;
            }

            var valorTarifa = _configuration.GetValue<decimal>("Tarifa:ValorTransferencia", 2.00m);

            var connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=/app/data/bankmore.db";
            
            var connectionFactory = new SqliteConnectionFactory(connectionString);
            var tarifaRepository = new TarifaRepository(connectionFactory);

            var tarifa = new Tarifa
            {
                IdTarifa = Guid.NewGuid().ToString(),
                IdContaCorrente = transferencia.IdContaCorrenteOrigem,
                DataMovimento = DateTime.Now,
                Valor = valorTarifa
            };

            await tarifaRepository.CreateAsync(tarifa);

            var tarifacaoRealizada = new TarifacaoRealizadaEvent
            {
                IdContaCorrente = transferencia.IdContaCorrenteOrigem,
                ValorTarifado = valorTarifa,
                DataTarifacao = DateTime.Now
            };

            var messageJson = JsonSerializer.Serialize(tarifacaoRealizada);
            await _producer.ProduceAsync("tarifacoes-realizadas", Guid.NewGuid().ToString(), messageJson);

            _logger.LogInformation("Tarifa processada com sucesso para conta {conta}, valor {valor}", 
                transferencia.IdContaCorrenteOrigem, valorTarifa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar transferência realizada: {message}", message);
            throw;
        }
    }
}

