using BankMore.Domain.Commands;
using BankMore.Domain.Common;
using BankMore.Domain.Entities;
using BankMore.Domain.Events;
using BankMore.Domain.Repositories;
using BankMore.Domain.Services;
using MediatR;
using System.Text.Json;

namespace BankMore.Domain.Handlers;

public class TransferenciaHandler : IRequestHandler<TransferenciaCommand, ApiResponse>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IMovimentoRepository _movimentoRepository;
    private readonly ITransferenciaRepository _transferenciaRepository;
    private readonly IIdempotenciaRepository _idempotenciaRepository;
    private readonly IKafkaProducerService _kafkaProducerService;

    public TransferenciaHandler(
        IContaCorrenteRepository contaRepository,
        IMovimentoRepository movimentoRepository,
        ITransferenciaRepository transferenciaRepository,
        IIdempotenciaRepository idempotenciaRepository,
        IKafkaProducerService kafkaProducerService)
    {
        _contaRepository = contaRepository;
        _movimentoRepository = movimentoRepository;
        _transferenciaRepository = transferenciaRepository;
        _idempotenciaRepository = idempotenciaRepository;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task<ApiResponse> Handle(TransferenciaCommand request, CancellationToken cancellationToken)
    {
        var idempotenciaExistente = await _idempotenciaRepository.GetByChaveAsync(request.ChaveIdempotencia);
        if (idempotenciaExistente != null)
        {
            var resultadoAnterior = JsonSerializer.Deserialize<ApiResponse>(idempotenciaExistente.Resultado);
            return resultadoAnterior ?? ApiResponse.SuccessResult();
        }

        var contaOrigem = await _contaRepository.GetByIdAsync(request.IdContaCorrenteOrigem);
        if (contaOrigem == null)
        {
            var errorResult = ApiResponse.ErrorResult("Conta origem não encontrada", ErrorTypes.INVALID_ACCOUNT);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        if (!contaOrigem.Ativo)
        {
            var errorResult = ApiResponse.ErrorResult("Conta origem inativa", ErrorTypes.INACTIVE_ACCOUNT);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        var contaDestino = await _contaRepository.GetByNumeroAsync(request.NumeroContaDestino);
        if (contaDestino == null)
        {
            var errorResult = ApiResponse.ErrorResult("Conta destino não encontrada", ErrorTypes.INVALID_ACCOUNT);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        if (!contaDestino.Ativo)
        {
            var errorResult = ApiResponse.ErrorResult("Conta destino inativa", ErrorTypes.INACTIVE_ACCOUNT);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        if (request.Valor <= 0)
        {
            var errorResult = ApiResponse.ErrorResult("Valor deve ser positivo", ErrorTypes.INVALID_VALUE);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        var saldoOrigem = await _movimentoRepository.GetSaldoByContaAsync(request.IdContaCorrenteOrigem);
        if (saldoOrigem < request.Valor)
        {
            var errorResult = ApiResponse.ErrorResult("Saldo insuficiente", ErrorTypes.INSUFFICIENT_FUNDS);
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }

        try
        {
            var movimentoDebito = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = request.IdContaCorrenteOrigem,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'D',
                Valor = request.Valor
            };
            await _movimentoRepository.CreateAsync(movimentoDebito);

            var movimentoCredito = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = contaDestino.IdContaCorrente,
                DataMovimento = DateTime.Now,
                TipoMovimento = 'C',
                Valor = request.Valor
            };
            await _movimentoRepository.CreateAsync(movimentoCredito);

            var transferencia = new Transferencia
            {
                IdTransferencia = Guid.NewGuid().ToString(),
                IdContaCorrenteOrigem = request.IdContaCorrenteOrigem,
                IdContaCorrenteDestino = contaDestino.IdContaCorrente,
                DataMovimento = DateTime.Now,
                Valor = request.Valor
            };
            await _transferenciaRepository.CreateAsync(transferencia);

            var evento = new TransferenciaRealizadaEvent
            {
                ChaveIdempotencia = request.ChaveIdempotencia,
                IdContaCorrenteOrigem = request.IdContaCorrenteOrigem,
                Valor = request.Valor,
                DataTransferencia = DateTime.Now
            };
            await _kafkaProducerService.PublishTransferenciaRealizadaAsync(evento);

            var successResult = ApiResponse.SuccessResult();
            await SalvarIdempotencia(request.ChaveIdempotencia, request, successResult);

            return successResult;
        }
        catch (Exception ex)
        {
            var errorResult = ApiResponse.ErrorResult($"Erro interno: {ex.Message}", "INTERNAL_ERROR");
            await SalvarIdempotencia(request.ChaveIdempotencia, request, errorResult);
            return errorResult;
        }
    }

    private async Task SalvarIdempotencia(string chave, TransferenciaCommand request, ApiResponse resultado)
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

