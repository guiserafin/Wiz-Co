namespace WizCo.Api.DTOs.Common;

public sealed class PaginacaoRequest
{
    public const int TamanhoPaginaPadrao = 10;
    public const int TamanhoPaginaMaximo = 100;

    private readonly int _pagina = 1;
    private readonly int _tamanhoPagina = TamanhoPaginaPadrao;

    public int Pagina
    {
        get => _pagina;
        init => _pagina = value < 1 ? 1 : value;
    }

    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        init => _tamanhoPagina = value switch
        {
            < 1 => TamanhoPaginaPadrao,
            > TamanhoPaginaMaximo => TamanhoPaginaMaximo,
            _ => value
        };
    }
}
