namespace WizCo.Api.Tests.Validators;

using FluentAssertions;
using WizCo.Api.DTOs.Common;

public class PaginacaoRequestTests
{
    [Fact]
    public void Padrao_DeveTerPagina1ETamanho10()
    {
        var paginacao = new PaginacaoRequest();

        paginacao.Pagina.Should().Be(1);
        paginacao.TamanhoPagina.Should().Be(PaginacaoRequest.TamanhoPaginaPadrao);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Pagina_ComValorInvalido_DeveSerClampedParaUm(int valorInvalido)
    {
        var paginacao = new PaginacaoRequest { Pagina = valorInvalido };

        paginacao.Pagina.Should().Be(1);
    }

    [Fact]
    public void TamanhoPagina_AcimaDoMaximo_DeveSerClampedParaMaximo()
    {
        var paginacao = new PaginacaoRequest { TamanhoPagina = 500 };

        paginacao.TamanhoPagina.Should().Be(PaginacaoRequest.TamanhoPaginaMaximo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void TamanhoPagina_ComValorInvalido_DeveUsarPadrao(int valorInvalido)
    {
        var paginacao = new PaginacaoRequest { TamanhoPagina = valorInvalido };

        paginacao.TamanhoPagina.Should().Be(PaginacaoRequest.TamanhoPaginaPadrao);
    }

    [Fact]
    public void TamanhoPagina_DentroDoIntervalo_DeveSerMantido()
    {
        var paginacao = new PaginacaoRequest { TamanhoPagina = 25 };

        paginacao.TamanhoPagina.Should().Be(25);
    }
}
