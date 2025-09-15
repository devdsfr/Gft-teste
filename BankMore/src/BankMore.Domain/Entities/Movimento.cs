namespace BankMore.Domain.Entities;

public class Movimento
{
    public string IdMovimento { get; set; } = string.Empty;
    public string IdContaCorrente { get; set; } = string.Empty;
    public DateTime DataMovimento { get; set; }
    public char TipoMovimento { get; set; } // 'C' = Crédito, 'D' = Débito
    public decimal Valor { get; set; }
}

