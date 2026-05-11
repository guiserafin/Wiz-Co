namespace WizCo.Api.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using WizCo.Api.DTOs.Common;
using WizCo.Api.DTOs.Requests;
using WizCo.Api.DTOs.Responses;
using WizCo.Api.Entities;
using WizCo.Api.Repositories.Interfaces;
using WizCo.Api.Services.Interfaces;

public sealed class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(IPedidoRepository repository, IMapper mapper, ILogger<PedidoService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PedidoResponse> CriarAsync(CriarPedidoRequest request)
    {
        _logger.LogInformation(
            "Criando pedido para o cliente {ClienteNome} com {QuantidadeItens} item(ns)",
            request.ClienteNome, request.Itens.Count);

        var itens = request.Itens
            .Select(i => new ItemPedido(i.ProdutoNome, i.Quantidade, i.PrecoUnitario))
            .ToList();

        var pedido = new Pedido(request.ClienteNome, itens);

        await _repository.AdicionarAsync(pedido);

        _logger.LogInformation(
            "Pedido {PedidoId} criado com sucesso. ValorTotal: {ValorTotal}",
            pedido.Id, pedido.ValorTotal);

        return _mapper.Map<PedidoResponse>(pedido);
    }

    public async Task<PedidoDetalhadoResponse> ObterPorIdAsync(Guid id)
    {
        var pedido = await _repository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido {id} não encontrado.");

        return _mapper.Map<PedidoDetalhadoResponse>(pedido);
    }

    public async Task<PaginacaoResponse<PedidoDetalhadoResponse>> ListarAsync(
        PedidoStatus? status, PaginacaoRequest paginacao)
    {
        var (pedidos, total) = await _repository.ListarAsync(
            status, paginacao.Pagina, paginacao.TamanhoPagina);

        var itens = pedidos
            .Select(p => _mapper.Map<PedidoDetalhadoResponse>(p))
            .ToList();

        return PaginacaoResponse<PedidoDetalhadoResponse>.Criar(
            itens, paginacao.Pagina, paginacao.TamanhoPagina, total);
    }

    public async Task CancelarAsync(Guid id)
    {
        _logger.LogInformation("Tentando cancelar pedido {PedidoId}", id);

        var pedido = await _repository.ObterPorIdParaEdicaoAsync(id)
            ?? throw new KeyNotFoundException($"Pedido {id} não encontrado.");

        pedido.Cancelar();

        await _repository.AtualizarAsync(pedido);

        _logger.LogInformation("Pedido {PedidoId} cancelado com sucesso.", id);
    }

    public async Task PagarAsync(Guid id)
    {
        _logger.LogInformation("Tentando pagar pedido {PedidoId}", id);

        var pedido = await _repository.ObterPorIdParaEdicaoAsync(id)
            ?? throw new KeyNotFoundException($"Pedido {id} não encontrado.");

        pedido.Pagar();

        await _repository.AtualizarAsync(pedido);

        _logger.LogInformation("Pedido {PedidoId} pago com sucesso.", id);
    }
}
