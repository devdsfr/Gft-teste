using BankMore.Domain.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BankMore.Infrastructure.Services;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
    }

    public async Task PublishTransferenciaRealizadaAsync<T>(T message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        _logger.LogInformation("Publicando transferência realizada: {message}", messageJson);
        
        await Task.Delay(100);
    }
}

