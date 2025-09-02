using eAgenda.Core.Aplicacao.ModuloContato.Commands;
using eAgenda.WebApi.Models.ModuloContato;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApi.Controllers;

[ApiController]
[Route("contatos")]
public class ContatoController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CadastrarContatoResponse>> Cadastrar(CadastrarContatoRequest request)
    {
        var command = new CadastrarContatoCommand(
             request.Nome,
             request.Telefone,
             request.Email,
             request.Empresa,
             request.Cargo
        );

        var result = await mediator.Send(command);

        if (result.IsFailed)
            return BadRequest();

        var response = new CadastrarContatoResponse(result.Value.Id);

        return CreatedAtAction(nameof(SelecionarRegistroPorId), new { contatoId = result.Value.Id }, response);
    }

    [HttpPut("{contatoId:guid}")]
    public async Task<ActionResult<EditarContatoResponse>> Editar(Guid contatoId, EditarContatoRequest request)
    {
        var command = new EditarContatoCommand(
            contatoId,
            request.Nome,
            request.Telefone,
            request.Email,
            request.Empresa,
            request.Cargo
        );

        var result = await mediator.Send(command);

        if (result.IsFailed)
            return BadRequest();

        var response = new EditarContatoResponse(
            result.Value.Nome,
            result.Value.Telefone,
            result.Value.Email,
            result.Value.Empresa,
            result.Value.Cargo
        );

        return Ok(response);
    }

    [HttpDelete("{contatoId:guid}")]
    public async Task<ActionResult<ExcluirContatoResponse>> Excluir(Guid contatoId)
    {
        var command = new ExcluirContatoCommand(contatoId);

        var result = await mediator.Send(command);

        if (result.IsFailed)
            return BadRequest();

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<SelecionarContatosResponse>> SelecionarRegistros(
        [FromQuery] SelecionarContatosRequest? selecionarContatosRequest
    )
    {
        var query = new SelecionarContatosQuery(selecionarContatosRequest?.Quantidade);

        var result = await mediator.Send(query);

        if (result.IsFailed)
            return BadRequest();

        var response = new SelecionarContatosResponse(
            result.Value.Contatos.Count,
            result.Value.Contatos
        );

        return Ok(response);
    }

    [HttpGet("{contatoId:guid}")]
    public async Task<ActionResult<SelecionarContatoPorIdResponse>> SelecionarRegistroPorId(Guid contatoId)
    {
        var query = new SelecionarContatoPorIdQuery(contatoId);

        var result = await mediator.Send(query);

        if (result.IsFailed)
            return NotFound(contatoId);

        var response = new SelecionarContatoPorIdResponse(
            result.Value.Id,
            result.Value.Nome,
            result.Value.Telefone,
            result.Value.Email,
            result.Value.Empresa,
            result.Value.Cargo,
            result.Value.Compromissos
        );

        return Ok(response);
    }
}