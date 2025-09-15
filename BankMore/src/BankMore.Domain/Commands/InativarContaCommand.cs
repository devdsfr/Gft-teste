using MediatR;
using BankMore.Domain.Common;

namespace BankMore.Domain.Commands;

public class InativarContaCommand : IRequest<ApiResponse>
{
    public string IdContaCorrente { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

