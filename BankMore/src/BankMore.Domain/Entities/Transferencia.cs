namespace BankMore.Domain.Entities;

public class Transferencia
{
    public string IdTransferencia { get; set; } = string.Empty;
    public string IdContaCorrenteOrigem { get; set; } = string.Empty;
    public string IdContaCorrenteDestino { get; set; } = string.Empty;
    public DateTime DataMovimento { get; set; }
    public decimal Valor { get; set; }
}

