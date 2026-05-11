namespace WizCo.Api.Services.Interfaces;

using WizCo.Api.DTOs.Common;
using WizCo.Api.DTOs.Requests;
using WizCo.Api.DTOs.Responses;
using WizCo.Api.Entities;

public interface IPedidoService
{
    Task<PedidoResponse> CriarAsync(CriarPedidoRequest request);
    Task<PedidoDetalhadoResponse> ObterPorIdAsync(Guid id);
    Task<PaginacaoResponse<PedidoDetalhadoResponse>> ListarAsync(
        PedidoStatus? status, PaginacaoRequest paginacao);
    Task CancelarAsync(Guid id);
    Task PagarAsync(Guid id);
}
