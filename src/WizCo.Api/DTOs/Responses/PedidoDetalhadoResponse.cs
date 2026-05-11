namespace WizCo.Api.DTOs.Responses;

public sealed record PedidoDetalhadoResponse(
    Guid Id,
    string ClienteNome,
    DateTime DataCriacao,
    string Status,
    decimal ValorTotal,
    IReadOnlyCollection<ItemPedidoResponse> Itens);
