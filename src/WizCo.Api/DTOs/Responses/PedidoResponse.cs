namespace WizCo.Api.DTOs.Responses;

public sealed record PedidoResponse(
    Guid Id,
    string ClienteNome,
    string Status,
    decimal ValorTotal);
