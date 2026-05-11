namespace WizCo.Api.DTOs.Requests;

public sealed class CriarPedidoRequest
{
    public string ClienteNome { get; init; } = string.Empty;
    public List<CriarItemPedidoRequest> Itens { get; init; } = new();
}
