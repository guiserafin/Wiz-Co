namespace WizCo.Api.Repositories.Interfaces;

using WizCo.Api.Entities;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id);
    Task<Pedido?> ObterPorIdParaEdicaoAsync(Guid id);
    Task<(IReadOnlyList<Pedido> Itens, int Total)> ListarAsync(
        PedidoStatus? status, int pagina, int tamanhoPagina);
    Task AdicionarAsync(Pedido pedido);
    Task AtualizarAsync(Pedido pedido);
}
