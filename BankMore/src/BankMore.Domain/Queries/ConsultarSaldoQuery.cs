using MediatR;
using BankMore.Domain.Common;

namespace BankMore.Domain.Queries;

public class ConsultarSaldoQuery : IRequest<ApiResponse<ConsultarSaldoResponse>>
{
    public string IdContaCorrente { get; set; } = string.Empty;
}

public class ConsultarSaldoResponse
{
    public int NumeroContaCorrente { get; set; }
    public string NomeTitular { get; set; } = string.Empty;
    public DateTime DataHoraConsulta { get; set; }
    public decimal SaldoAtual { get; set; }
}

