namespace WizCo.Api.Tests.Services;

using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WizCo.Api.DTOs.Common;
using WizCo.Api.DTOs.Requests;
using WizCo.Api.Entities;
using WizCo.Api.Mappings;
using WizCo.Api.Repositories.Interfaces;
using WizCo.Api.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly PedidoService _sut;

    public PedidoServiceTests()
    {
        _repositoryMock = new Mock<IPedidoRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<PedidoProfile>());
        _mapper = config.CreateMapper();

        var logger = Mock.Of<ILogger<PedidoService>>();

        _sut = new PedidoService(_repositoryMock.Object, _mapper, logger);
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveRetornarPedidoResponse()
    {
        var request = new CriarPedidoRequest
        {
            ClienteNome = "João Silva",
            Itens = new List<CriarItemPedidoRequest>
            {
                new() { ProdutoNome = "Notebook", Quantidade = 1, PrecoUnitario = 3500m }
            }
        };

        _repositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Pedido>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.CriarAsync(request);

        result.Should().NotBeNull();
        result.ClienteNome.Should().Be("João Silva");
        result.ValorTotal.Should().Be(3500m);
        result.Status.Should().Be("Novo");
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CriarAsync_ComMultiplosItens_DeveCalcularValorTotalCorretamente()
    {
        var request = new CriarPedidoRequest
        {
            ClienteNome = "Maria",
            Itens = new List<CriarItemPedidoRequest>
            {
                new() { ProdutoNome = "Mouse", Quantidade = 2, PrecoUnitario = 50m },
                new() { ProdutoNome = "Teclado", Quantidade = 1, PrecoUnitario = 150m }
            }
        };

        _repositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Pedido>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.CriarAsync(request);

        result.ValorTotal.Should().Be(250m);
    }

    [Fact]
    public async Task CriarAsync_DevePersistirPedido_UmaVez()
    {
        var request = new CriarPedidoRequest
        {
            ClienteNome = "Teste",
            Itens = new List<CriarItemPedidoRequest>
            {
                new() { ProdutoNome = "Produto", Quantidade = 1, PrecoUnitario = 10m }
            }
        };

        _repositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Pedido>()))
            .Returns(Task.CompletedTask);

        await _sut.CriarAsync(request);

        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveLancarKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.ObterPorIdAsync(id))
            .ReturnsAsync((Pedido?)null);

        var act = async () => await _sut.ObterPorIdAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact]
    public async Task CancelarAsync_ComPedidoPago_DeveLancarInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var pedidoPago = CriarPedidoPagoMock();

        _repositoryMock
            .Setup(r => r.ObterPorIdParaEdicaoAsync(id))
            .ReturnsAsync(pedidoPago);

        var act = async () => await _sut.CancelarAsync(id);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Pago*");
    }

    [Fact]
    public async Task CancelarAsync_ComPedidoInexistente_DeveLancarKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.ObterPorIdParaEdicaoAsync(id))
            .ReturnsAsync((Pedido?)null);

        var act = async () => await _sut.CancelarAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CancelarAsync_ComPedidoNovo_DeveAtualizarStatus()
    {
        var id = Guid.NewGuid();
        var pedido = CriarPedidoNovoMock();

        _repositoryMock
            .Setup(r => r.ObterPorIdParaEdicaoAsync(id))
            .ReturnsAsync(pedido);

        _repositoryMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Pedido>()))
            .Returns(Task.CompletedTask);

        await _sut.CancelarAsync(id);

        _repositoryMock.Verify(r => r.AtualizarAsync(
            It.Is<Pedido>(p => p.Status == PedidoStatus.Cancelado)), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarPaginacaoComMetadados()
    {
        var pedidos = new List<Pedido>
        {
            CriarPedidoNovoMock(),
            CriarPedidoNovoMock()
        };
        var paginacao = new PaginacaoRequest { Pagina = 1, TamanhoPagina = 10 };

        _repositoryMock
            .Setup(r => r.ListarAsync(null, 1, 10))
            .ReturnsAsync((pedidos, 25));

        var result = await _sut.ListarAsync(null, paginacao);

        result.Should().NotBeNull();
        result.Itens.Should().HaveCount(2);
        result.Pagina.Should().Be(1);
        result.TamanhoPagina.Should().Be(10);
        result.TotalItens.Should().Be(25);
        result.TotalPaginas.Should().Be(3);
        result.TemPaginaAnterior.Should().BeFalse();
        result.TemProximaPagina.Should().BeTrue();
    }

    [Fact]
    public async Task ListarAsync_SemResultados_DeveRetornarListaVaziaComTotalZero()
    {
        var paginacao = new PaginacaoRequest();
        _repositoryMock
            .Setup(r => r.ListarAsync(It.IsAny<PedidoStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Array.Empty<Pedido>(), 0));

        var result = await _sut.ListarAsync(null, paginacao);

        result.Itens.Should().BeEmpty();
        result.TotalItens.Should().Be(0);
        result.TotalPaginas.Should().Be(0);
    }

    [Fact]
    public async Task ListarAsync_DevePropagarFiltroDeStatusParaRepository()
    {
        var paginacao = new PaginacaoRequest { Pagina = 2, TamanhoPagina = 5 };
        _repositoryMock
            .Setup(r => r.ListarAsync(PedidoStatus.Pago, 2, 5))
            .ReturnsAsync((Array.Empty<Pedido>(), 0));

        await _sut.ListarAsync(PedidoStatus.Pago, paginacao);

        _repositoryMock.Verify(
            r => r.ListarAsync(PedidoStatus.Pago, 2, 5), Times.Once);
    }

    [Fact]
    public async Task PagarAsync_ComPedidoInexistente_DeveLancarKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.ObterPorIdParaEdicaoAsync(id))
            .ReturnsAsync((Pedido?)null);

        var act = async () => await _sut.PagarAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task PagarAsync_ComPedidoCancelado_DeveLancarInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var pedidoCancelado = CriarPedidoCanceladoMock();

        _repositoryMock
            .Setup(r => r.ObterPorIdParaEdicaoAsync(id))
            .ReturnsAsync(pedidoCancelado);

        var act = async () => await _sut.PagarAsync(id);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cancelado*");
    }

    [Fact]
    public async Task PagarAsync_ComPedidoJaPago_DeveLancarInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var pedidoPago = CriarPedidoPagoMock();

        _repositoryMock
            .Setup(r => r.ObterPorIdParaEdicaoAsync(id))
            .ReturnsAsync(pedidoPago);

        var act = async () => await _sut.PagarAsync(id);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Pago*");
    }

    [Fact]
    public async Task PagarAsync_ComPedidoNovo_DeveAtualizarStatusParaPago()
    {
        var id = Guid.NewGuid();
        var pedido = CriarPedidoNovoMock();

        _repositoryMock
            .Setup(r => r.ObterPorIdParaEdicaoAsync(id))
            .ReturnsAsync(pedido);

        _repositoryMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Pedido>()))
            .Returns(Task.CompletedTask);

        await _sut.PagarAsync(id);

        _repositoryMock.Verify(r => r.AtualizarAsync(
            It.Is<Pedido>(p => p.Status == PedidoStatus.Pago)), Times.Once);
    }

    private static Pedido CriarPedidoNovoMock()
    {
        var itens = new List<ItemPedido> { new("Produto Teste", 1, 100m) };
        return new Pedido("Cliente Teste", itens);
    }

    private static Pedido CriarPedidoPagoMock()
    {
        var itens = new List<ItemPedido> { new("Produto", 1, 100m) };
        var pedido = new Pedido("Cliente Pago", itens);
        typeof(Pedido)
            .GetProperty(nameof(Pedido.Status))!
            .SetValue(pedido, PedidoStatus.Pago);
        return pedido;
    }

    private static Pedido CriarPedidoCanceladoMock()
    {
        var itens = new List<ItemPedido> { new("Produto", 1, 100m) };
        var pedido = new Pedido("Cliente Cancelado", itens);
        typeof(Pedido)
            .GetProperty(nameof(Pedido.Status))!
            .SetValue(pedido, PedidoStatus.Cancelado);
        return pedido;
    }
}
