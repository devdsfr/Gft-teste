using BankMore.Domain.Commands;
using BankMore.Domain.Queries;
using BankMore.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankMore.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ContasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtService _jwtService;

    public ContasController(IMediator mediator, IJwtService jwtService)
    {
        _mediator = mediator;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Cadastra uma nova conta corrente
    /// </summary>
    /// <param name="request">Dados para cadastro da conta</param>
    /// <returns>Número da conta criada</returns>
    [HttpPost("cadastrar")]
    public async Task<IActionResult> CadastrarConta([FromBody] CadastrarContaRequest request)
    {
        var command = new CadastrarContaCommand
        {
            Cpf = request.Cpf,
            Nome = request.Nome,
            Senha = request.Senha
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage, type = result.ErrorType });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Realiza login na conta corrente
    /// </summary>
    /// <param name="request">Dados para login</param>
    /// <returns>Token JWT</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand
        {
            CpfOuNumeroConta = request.CpfOuNumeroConta,
            Senha = request.Senha
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.ErrorMessage, type = result.ErrorType });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Inativa uma conta corrente
    /// </summary>
    /// <param name="request">Dados para inativação</param>
    /// <returns>Status da operação</returns>
    [HttpPost("inativar")]
    [Authorize]
    public async Task<IActionResult> InativarConta([FromBody] InativarContaRequest request)
    {
        var contaId = GetContaIdFromToken();
        if (string.IsNullOrEmpty(contaId))
        {
            return Forbid();
        }

        var command = new InativarContaCommand
        {
            IdContaCorrente = contaId,
            Senha = request.Senha
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage, type = result.ErrorType });
        }

        return NoContent();
    }

    /// <summary>
    /// Consulta o saldo da conta corrente
    /// </summary>
    /// <returns>Dados do saldo</returns>
    [HttpGet("saldo")]
    [Authorize]
    public async Task<IActionResult> ConsultarSaldo()
    {
        var contaId = GetContaIdFromToken();
        if (string.IsNullOrEmpty(contaId))
        {
            return Forbid();
        }

        var query = new ConsultarSaldoQuery
        {
            IdContaCorrente = contaId
        };

        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage, type = result.ErrorType });
        }

        return Ok(result.Data);
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

public class CadastrarContaRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string CpfOuNumeroConta { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class InativarContaRequest
{
    public string Senha { get; set; } = string.Empty;
}

