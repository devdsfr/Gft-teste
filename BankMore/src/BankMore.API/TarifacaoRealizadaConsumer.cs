using BankMore.Domain.Entities;
using BankMore.Domain.Events;
using BankMore.Domain.Repositories;
using KafkaFlow;
using System.Text.Json;

namespace BankMore.API;

public class TarifacaoRealizadaConsumer : IMessageHandler<string>
{
    private readonly ILogger<TarifacaoRealizadaConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TarifacaoRealizadaConsumer(
        ILogger<TarifacaoRealizadaConsumer> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
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

            using var scope = _serviceProvider.CreateScope();
            var movimentoRepository = scope.ServiceProvider.GetRequiredService<IMovimentoRepository>();

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

