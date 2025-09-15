namespace BankMore.Domain.Events;

public class TarifacaoRealizadaEvent
{
    public string IdContaCorrente { get; set; } = string.Empty;
    public decimal ValorTarifado { get; set; }
    public DateTime DataTarifacao { get; set; }
}

