using BankMore.Domain.Commands;
using BankMore.Domain.Common;
using BankMore.Domain.Entities;
using BankMore.Domain.Repositories;
using BankMore.Domain.Services;
using MediatR;

namespace BankMore.Domain.Handlers;

public class CadastrarContaHandler : IRequestHandler<CadastrarContaCommand, ApiResponse<CadastrarContaResponse>>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly ICpfValidator _cpfValidator;
    private readonly IPasswordHasher _passwordHasher;

    public CadastrarContaHandler(
        IContaCorrenteRepository contaRepository,
        ICpfValidator cpfValidator,
        IPasswordHasher passwordHasher)
    {
        _contaRepository = contaRepository;
        _cpfValidator = cpfValidator;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<CadastrarContaResponse>> Handle(CadastrarContaCommand request, CancellationToken cancellationToken)
    {
        if (!_cpfValidator.IsValid(request.Cpf))
        {
            return ApiResponse<CadastrarContaResponse>.ErrorResult(
                "CPF inválido", 
                ErrorTypes.INVALID_DOCUMENT);
        }

        if (await _contaRepository.ExistsByCpfAsync(request.Cpf))
        {
            return ApiResponse<CadastrarContaResponse>.ErrorResult(
                "Já existe uma conta cadastrada com este CPF", 
                ErrorTypes.ACCOUNT_ALREADY_EXISTS);
        }

        var (hash, salt) = _passwordHasher.HashPassword(request.Senha);

        var conta = new ContaCorrente
        {
            IdContaCorrente = Guid.NewGuid().ToString(),
            Numero = await _contaRepository.GetNextNumeroContaAsync(),
            Cpf = request.Cpf.Replace(".", "").Replace("-", "").Replace(" ", ""),
            Nome = request.Nome,
            Ativo = true,
            Senha = hash,
            Salt = salt,
            DataCriacao = DateTime.Now
        };

        await _contaRepository.CreateAsync(conta);

        return ApiResponse<CadastrarContaResponse>.SuccessResult(new CadastrarContaResponse
        {
            NumeroConta = conta.Numero
        });
    }
}

