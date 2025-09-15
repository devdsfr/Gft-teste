using MediatR;
using BankMore.Domain.Common;

namespace BankMore.Domain.Commands;

public class TransferenciaCommand : IRequest<ApiResponse>
{
    public string ChaveIdempotencia { get; set; } = string.Empty;
    public string IdContaCorrenteOrigem { get; set; } = string.Empty;
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
}

