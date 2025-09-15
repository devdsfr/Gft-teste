using BankMore.Domain.Commands;
using BankMore.Domain.Common;
using BankMore.Domain.Entities;
using BankMore.Domain.Repositories;
using MediatR;
using System.Text.Json;

namespace BankMore.Domain.Handlers;

public class MovimentacaoHandler : IRequestHandler<MovimentacaoCommand, ApiResponse>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IMovimentoRepository _movimentoRepository;
    private readonly IIdempotenciaRepository _idempotenciaRepository;

    public MovimentacaoHandler(
        IContaCorrenteRepository contaRepository,
        IMovimentoRepository movimentoRepository,
        IIdempotenciaRepository idempotenciaRepository)
    {
        _contaRepository = contaRepository;
        _movimentoRepository = movimentoRepository;
        _idempotenciaRepository = idempotenciaRepository;
    }

    public async Task<ApiResponse> Handle(MovimentacaoCommand request, CancellationToken cancellationToken)
    {
        var idempotenciaExistente = await _idempotenciaRepository.GetByChaveAsync(request.ChaveIdempotencia);
        if (idempotenciaExistente != null)
        {
            var resultadoAnterior = JsonSerializer.Deserialize<ApiResponse>(idempotenciaExistente.Resultado);
            return resultadoAnterior ?? ApiResponse.SuccessResult();
        }

        var idContaCorrente = request.IdContaCorrente;
        if (request.NumeroContaCorrente.HasValue)
        {
            var contaPorNumero = await _contaRepository.GetByNumeroAsync(request.NumeroContaCorrente.Value);
            if (contaPorNumero == null)
            {
                var errorResult = ApiResponse.ErrorResult("Conta corrente não encontrada", ErrorTypes.INVALID_ACCOUNT);
                await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
                return errorResult;
            }
            idContaCorrente = contaPorNumero.IdContaCorrente;
        }

        var conta = await _contaRepository.GetByIdAsync(idContaCorrente);
        if (conta == null)
        {
            var errorResult = ApiResponse.ErrorResult("Conta corrente não encontrada", ErrorTypes.INVALID_ACCOUNT);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        if (!conta.Ativo)
        {
            var errorResult = ApiResponse.ErrorResult("Conta corrente inativa", ErrorTypes.INACTIVE_ACCOUNT);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        if (request.Valor <= 0)
        {
            var errorResult = ApiResponse.ErrorResult("Valor deve ser positivo", ErrorTypes.INVALID_VALUE);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
        {
            var errorResult = ApiResponse.ErrorResult("Tipo de movimento inválido", ErrorTypes.INVALID_TYPE);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        if (request.NumeroContaCorrente.HasValue && 
            request.NumeroContaCorrente.Value != conta.Numero && 
            request.TipoMovimento != 'C')
        {
            var errorResult = ApiResponse.ErrorResult("Apenas créditos são permitidos para contas diferentes", ErrorTypes.INVALID_TYPE);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        if (request.TipoMovimento == 'D')
        {
            var saldoAtual = await _movimentoRepository.GetSaldoByContaAsync(idContaCorrente);
            if (saldoAtual < request.Valor)
            {
                var errorResult = ApiResponse.ErrorResult("Saldo insuficiente", ErrorTypes.INSUFFICIENT_FUNDS);
                await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
                return errorResult;
            }
        }

        var movimento = new Movimento
        {
            IdMovimento = Guid.NewGuid().ToString(),
            IdContaCorrente = idContaCorrente,
            DataMovimento = DateTime.Now,
            TipoMovimento = request.TipoMovimento,
            Valor = request.Valor
        };

        await _movimentoRepository.CreateAsync(movimento);

        var successResult = ApiResponse.SuccessResult();
        await SalvarIdempotencia(request.ChaveIdempotencia, request, successResult);

        return successResult;
    }

    private async Task SalvarIdempotencia(string chave, MovimentacaoCommand request, ApiResponse resultado)
    {
        var idempotencia = new Idempotencia
        {
            ChaveIdempotencia = chave,
            Requisicao = JsonSerializer.Serialize(request),
            Resultado = JsonSerializer.Serialize(resultado)
        };

        await _idempotenciaRepository.CreateAsync(idempotencia);
    }
}

