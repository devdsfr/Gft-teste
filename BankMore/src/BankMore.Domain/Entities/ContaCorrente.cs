namespace BankMore.Domain.Entities;

public class ContaCorrente
{
    public string IdContaCorrente { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public string Senha { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
}

