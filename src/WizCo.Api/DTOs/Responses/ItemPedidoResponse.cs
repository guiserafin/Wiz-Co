namespace WizCo.Api.DTOs.Responses;

public sealed record ItemPedidoResponse(
    string ProdutoNome,
    int Quantidade,
    decimal PrecoUnitario);
