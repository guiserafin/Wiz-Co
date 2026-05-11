namespace WizCo.Api.Entities;

public sealed class ItemPedido
{
    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public string ProdutoNome { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }

    public Pedido Pedido { get; private set; } = null!;

    private ItemPedido() { }

    public ItemPedido(string produtoNome, int quantidade, decimal precoUnitario)
    {
        Id = Guid.NewGuid();
        ProdutoNome = produtoNome;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    public decimal CalcularSubtotal() => Quantidade * PrecoUnitario;
}
