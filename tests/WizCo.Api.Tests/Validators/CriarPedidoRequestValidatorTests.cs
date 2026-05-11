namespace WizCo.Api.Tests.Validators;

using FluentAssertions;
using WizCo.Api.DTOs.Requests;
using WizCo.Api.Validators;

public class CriarPedidoRequestValidatorTests
{
    private readonly CriarPedidoRequestValidator _validator = new();

    private static CriarItemPedidoRequest ItemValido() =>
        new() { ProdutoNome = "Produto", Quantidade = 1, PrecoUnitario = 10m };

    [Fact]
    public void Validar_ComDadosValidos_DevePassar()
    {
        var request = new CriarPedidoRequest
        {
            ClienteNome = "João",
            Itens = new List<CriarItemPedidoRequest> { ItemValido() }
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validar_SemClienteNome_DeveRetornarErro()
    {
        var request = new CriarPedidoRequest
        {
            ClienteNome = "",
            Itens = new List<CriarItemPedidoRequest> { ItemValido() }
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(CriarPedidoRequest.ClienteNome));
    }

    [Fact]
    public void Validar_SemItens_DeveRetornarErro()
    {
        var request = new CriarPedidoRequest
        {
            ClienteNome = "João",
            Itens = new List<CriarItemPedidoRequest>()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(CriarPedidoRequest.Itens));
    }

    [Fact]
    public void Validar_ComQuantidadeZero_DeveRetornarErro()
    {
        var request = new CriarPedidoRequest
        {
            ClienteNome = "João",
            Itens = new List<CriarItemPedidoRequest>
            {
                new() { ProdutoNome = "Produto", Quantidade = 0, PrecoUnitario = 10m }
            }
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantidade"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validar_ComPrecoUnitarioInvalido_DeveRetornarErro(decimal preco)
    {
        var request = new CriarPedidoRequest
        {
            ClienteNome = "João",
            Itens = new List<CriarItemPedidoRequest>
            {
                new() { ProdutoNome = "Produto", Quantidade = 1, PrecoUnitario = preco }
            }
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("PrecoUnitario"));
    }

    [Fact]
    public void Validar_SemNomeDoProduto_DeveRetornarErro()
    {
        var request = new CriarPedidoRequest
        {
            ClienteNome = "João",
            Itens = new List<CriarItemPedidoRequest>
            {
                new() { ProdutoNome = "", Quantidade = 1, PrecoUnitario = 10m }
            }
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("ProdutoNome"));
    }
}
