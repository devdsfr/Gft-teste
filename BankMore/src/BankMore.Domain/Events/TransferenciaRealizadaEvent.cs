namespace BankMore.Domain.Events;

public class TransferenciaRealizadaEvent
{
    public string ChaveIdempotencia { get; set; } = string.Empty;
    public string IdContaCorrenteOrigem { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataTransferencia { get; set; }
}

