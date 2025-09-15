namespace BankMore.Domain.Entities;

public class Tarifa
{
    public string IdTarifa { get; set; } = string.Empty;
    public string IdContaCorrente { get; set; } = string.Empty;
    public DateTime DataMovimento { get; set; }
    public decimal Valor { get; set; }
}

