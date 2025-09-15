using BankMore.Domain.Commands;
using BankMore.Domain.Queries;
using BankMore.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class MovimentacoesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtService _jwtService;

    public MovimentacoesController(IMediator mediator, IJwtService jwtService)
    {
        _mediator = mediator;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Realiza uma movimentação na conta corrente
    /// </summary>
    /// <param name="request">Dados da movimentação</param>
    /// <returns>Status da operação</returns>
    [HttpPost]
    public async Task<IActionResult> RealizarMovimentacao([FromBody] MovimentacaoRequest request)
    {
        var contaId = GetContaIdFromToken();
        if (string.IsNullOrEmpty(contaId))
        {
            return Forbid();
        }

        var command = new MovimentacaoCommand
        {
            ChaveIdempotencia = request.ChaveIdempotencia,
            IdContaCorrente = contaId,
            NumeroContaCorrente = request.NumeroContaCorrente,
            Valor = request.Valor,
            TipoMovimento = request.TipoMovimento
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

public class MovimentacaoRequest
{
    public string ChaveIdempotencia { get; set; } = string.Empty;
    public int? NumeroContaCorrente { get; set; }
    public decimal Valor { get; set; }
    public char TipoMovimento { get; set; } // 'C' = Crédito, 'D' = Débito
}

