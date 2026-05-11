namespace WizCo.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using WizCo.Api.DTOs.Common;
using WizCo.Api.DTOs.Requests;
using WizCo.Api.Entities;
using WizCo.Api.Services.Interfaces;

[ApiController]
[Route("pedidos")]
[Produces("application/json")]
public sealed class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;

    public PedidosController(IPedidoService service) => _service = service;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoRequest request)
    {
        var response = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var response = await _service.ObterPorIdAsync(id);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] PedidoStatus? status,
        [FromQuery] PaginacaoRequest? paginacao)
    {
        var response = await _service.ListarAsync(status, paginacao ?? new PaginacaoRequest());
        return Ok(response);
    }

    [HttpPut("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        await _service.CancelarAsync(id);
        return NoContent();
    }

    [HttpPut("{id:guid}/pagar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Pagar(Guid id)
    {
        await _service.PagarAsync(id);
        return NoContent();
    }
}
