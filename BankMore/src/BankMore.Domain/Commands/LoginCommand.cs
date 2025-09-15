using MediatR;
using BankMore.Domain.Common;

namespace BankMore.Domain.Commands;

public class LoginCommand : IRequest<ApiResponse<LoginResponse>>
{
    public string CpfOuNumeroConta { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}

