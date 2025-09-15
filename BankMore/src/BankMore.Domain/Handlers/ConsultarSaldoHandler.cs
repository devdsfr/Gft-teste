using BankMore.Domain.Common;
using BankMore.Domain.Queries;
using BankMore.Domain.Repositories;
using MediatR;

namespace BankMore.Domain.Handlers;

public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, ApiResponse<ConsultarSaldoResponse>>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IMovimentoRepository _movimentoRepository;

    public ConsultarSaldoHandler(
        IContaCorrenteRepository contaRepository,
        IMovimentoRepository movimentoRepository)
    {
        _contaRepository = contaRepository;
        _movimentoRepository = movimentoRepository;
    }

    public async Task<ApiResponse<ConsultarSaldoResponse>> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.GetByIdAsync(request.IdContaCorrente);
        if (conta == null)
        {
            return ApiResponse<ConsultarSaldoResponse>.ErrorResult(
                "Conta corrente não encontrada", 
                ErrorTypes.INVALID_ACCOUNT);
        }

        if (!conta.Ativo)
        {
            return ApiResponse<ConsultarSaldoResponse>.ErrorResult(
                "Conta corrente inativa", 
                ErrorTypes.INACTIVE_ACCOUNT);
        }

        var saldo = await _movimentoRepository.GetSaldoByContaAsync(request.IdContaCorrente);

        var response = new ConsultarSaldoResponse
        {
            NumeroContaCorrente = conta.Numero,
            NomeTitular = conta.Nome,
            DataHoraConsulta = DateTime.Now,
            SaldoAtual = saldo
        };

        return ApiResponse<ConsultarSaldoResponse>.SuccessResult(response);
    }
}

