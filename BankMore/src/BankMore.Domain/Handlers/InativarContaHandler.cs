using BankMore.Domain.Commands;
using BankMore.Domain.Common;
using BankMore.Domain.Repositories;
using BankMore.Domain.Services;
using MediatR;

namespace BankMore.Domain.Handlers;

public class InativarContaHandler : IRequestHandler<InativarContaCommand, ApiResponse>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IPasswordHasher _passwordHasher;

    public InativarContaHandler(
        IContaCorrenteRepository contaRepository,
        IPasswordHasher passwordHasher)
    {
        _contaRepository = contaRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse> Handle(InativarContaCommand request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.GetByIdAsync(request.IdContaCorrente);

        if (conta == null)
        {
            return ApiResponse.ErrorResult(
                "Conta corrente não encontrada", 
                ErrorTypes.INVALID_ACCOUNT);
        }

        if (!_passwordHasher.VerifyPassword(request.Senha, conta.Senha, conta.Salt))
        {
            return ApiResponse.ErrorResult(
                "Senha inválida", 
                ErrorTypes.USER_UNAUTHORIZED);
        }

        conta.Ativo = false;
        await _contaRepository.UpdateAsync(conta);

        return ApiResponse.SuccessResult();
    }
}

