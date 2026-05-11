namespace WizCo.Api.DTOs.Common;

public sealed record PaginacaoResponse<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens,
    int TotalPaginas)
{
    public bool TemPaginaAnterior => Pagina > 1;
    public bool TemProximaPagina => Pagina < TotalPaginas;

    public static PaginacaoResponse<T> Criar(
        IReadOnlyList<T> itens, int pagina, int tamanhoPagina, int totalItens)
    {
        var totalPaginas = totalItens == 0
            ? 0
            : (int)Math.Ceiling(totalItens / (double)tamanhoPagina);

        return new PaginacaoResponse<T>(itens, pagina, tamanhoPagina, totalItens, totalPaginas);
    }
}
