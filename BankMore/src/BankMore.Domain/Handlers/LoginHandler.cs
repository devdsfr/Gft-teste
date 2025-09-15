using BankMore.Domain.Commands;
using BankMore.Domain.Common;
using BankMore.Domain.Repositories;
using BankMore.Domain.Services;
using MediatR;

namespace BankMore.Domain.Handlers;

public class LoginHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginHandler(
        IContaCorrenteRepository contaRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _contaRepository = contaRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var conta = await TryFindContaAsync(request.CpfOuNumeroConta);

        if (conta == null)
        {
            return ApiResponse<LoginResponse>.ErrorResult(
                "Usuário não autorizado", 
                ErrorTypes.USER_UNAUTHORIZED);
        }

        if (!_passwordHasher.VerifyPassword(request.Senha, conta.Senha, conta.Salt))
        {
            return ApiResponse<LoginResponse>.ErrorResult(
                "Usuário não autorizado", 
                ErrorTypes.USER_UNAUTHORIZED);
        }

        var token = _jwtService.GenerateToken(conta);

        return ApiResponse<LoginResponse>.SuccessResult(new LoginResponse
        {
            Token = token
        });
    }

    private async Task<Domain.Entities.ContaCorrente?> TryFindContaAsync(string cpfOuNumeroConta)
    {
        // Tentar como CPF primeiro
        var conta = await _contaRepository.GetByCpfAsync(cpfOuNumeroConta);
        
        if (conta != null)
            return conta;

        // Tentar como número da conta
        if (int.TryParse(cpfOuNumeroConta, out var numeroConta))
        {
            conta = await _contaRepository.GetByNumeroAsync(numeroConta);
        }

        return conta;
    }
}

