namespace BankMore.Domain.Services;

public interface IKafkaProducerService
{
    Task PublishTransferenciaRealizadaAsync<T>(T message);
}

