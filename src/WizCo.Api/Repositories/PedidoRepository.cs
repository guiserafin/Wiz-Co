namespace WizCo.Api.Repositories;

using Microsoft.EntityFrameworkCore;
using WizCo.Api.Data;
using WizCo.Api.Entities;
using WizCo.Api.Repositories.Interfaces;

public sealed class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context) => _context = context;

    public async Task<Pedido?> ObterPorIdAsync(Guid id)
        => await _context.Pedidos
            .Include(p => p.Itens)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Pedido?> ObterPorIdParaEdicaoAsync(Guid id)
        => await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<(IReadOnlyList<Pedido> Itens, int Total)> ListarAsync(
        PedidoStatus? status, int pagina, int tamanhoPagina)
    {
        var query = _context.Pedidos.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var total = await query.CountAsync();

        var itens = await query
            .OrderByDescending(p => p.DataCriacao)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Include(p => p.Itens)
            .ToListAsync();

        return (itens, total);
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync();
    }
}
