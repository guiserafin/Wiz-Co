namespace WizCo.Api.DTOs.Requests;

public sealed class CriarItemPedidoRequest
{
    public string ProdutoNome { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
}
