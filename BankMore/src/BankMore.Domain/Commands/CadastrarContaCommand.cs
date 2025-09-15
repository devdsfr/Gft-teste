using MediatR;
using BankMore.Domain.Common;

namespace BankMore.Domain.Commands;

public class CadastrarContaCommand : IRequest<ApiResponse<CadastrarContaResponse>>
{
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class CadastrarContaResponse
{
    public int NumeroConta { get; set; }
}

