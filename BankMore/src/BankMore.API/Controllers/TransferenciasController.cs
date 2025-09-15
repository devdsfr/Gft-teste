using BankMore.Domain.Commands;
using BankMore.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TransferenciasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtService _jwtService;

    public TransferenciasController(IMediator mediator, IJwtService jwtService)
    {
        _mediator = mediator;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Realiza uma transferência entre contas
    /// </summary>
    /// <param name="request">Dados da transferência</param>
    /// <returns>Status da operação</returns>
    [HttpPost]
    public async Task<IActionResult> RealizarTransferencia([FromBody] TransferenciaRequest request)
    {
        var contaId = GetContaIdFromToken();
        if (string.IsNullOrEmpty(contaId))
        {
            return Forbid();
        }

        var command = new TransferenciaCommand
        {
            ChaveIdempotencia = request.ChaveIdempotencia,
            IdContaCorrenteOrigem = contaId,
            NumeroContaDestino = request.NumeroContaDestino,
            Valor = request.Valor
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage, type = result.ErrorType });
        }

        return NoContent();
    }

    private string? GetContaIdFromToken()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            return _jwtService.GetContaIdFromToken(token);
        }
        return null;
    }
}

public class TransferenciaRequest
{
    public string ChaveIdempotencia { get; set; } = string.Empty;
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
}

