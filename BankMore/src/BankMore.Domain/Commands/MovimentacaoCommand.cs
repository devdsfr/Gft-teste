using MediatR;
using BankMore.Domain.Common;

namespace BankMore.Domain.Commands;

public class MovimentacaoCommand : IRequest<ApiResponse>
{
    public string ChaveIdempotencia { get; set; } = string.Empty;
    public string IdContaCorrente { get; set; } = string.Empty;
    public int? NumeroContaCorrente { get; set; }
    public decimal Valor { get; set; }
    public char TipoMovimento { get; set; } // 'C' = Crédito, 'D' = Débito
}

