namespace WizCo.Api.Entities;

public sealed class Pedido
{
    public Guid Id { get; private set; }
    public string ClienteNome { get; private set; } = string.Empty;
    public DateTime DataCriacao { get; private set; }
    public PedidoStatus Status { get; private set; }
    public decimal ValorTotal { get; private set; }
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    private readonly List<ItemPedido> _itens = new();

    private Pedido() { }

    public Pedido(string clienteNome, IEnumerable<ItemPedido> itens)
    {
        Id = Guid.NewGuid();
        ClienteNome = clienteNome;
        DataCriacao = DateTime.UtcNow;
        Status = PedidoStatus.Novo;
        _itens.AddRange(itens);
        ValorTotal = _itens.Sum(i => i.CalcularSubtotal());
    }

    public void Cancelar()
    {
        if (Status == PedidoStatus.Pago)
            throw new InvalidOperationException("Pedido com status Pago não pode ser cancelado.");

        Status = PedidoStatus.Cancelado;
    }

    public void Pagar()
    {
        if (Status == PedidoStatus.Cancelado)
            throw new InvalidOperationException("Pedido com status Cancelado não pode ser pago.");

        if (Status == PedidoStatus.Pago)
            throw new InvalidOperationException("Pedido já está com status Pago.");

        Status = PedidoStatus.Pago;
    }
}
