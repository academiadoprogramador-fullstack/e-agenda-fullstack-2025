using AutoMapper;
using eAgenda.Core.Aplicacao.ModuloAutenticacao.Commands;
using eAgenda.Core.Dominio.ModuloAutenticacao;
using eAgenda.WebApi.Identity;
using eAgenda.WebApi.Models.ModuloAutenticacao;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApi.Controllers;

[ApiController]
[Route("auth")]
public class AutenticacaoController(
    IMediator mediator,
    IMapper mapper
) : Controller
{
    [HttpPost("registrar")]
    public async Task<ActionResult<AccessToken>> Registrar(RegistrarUsuarioRequest request)
    {
        var command = mapper.Map<RegistrarUsuarioCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            if (result.HasError(e => e.HasMetadata("TipoErro", m => m.Equals("RequisicaoInvalida"))))
            {
                var errosDeValidacao = result.Errors
                    .SelectMany(e => e.Reasons.OfType<IError>())
                    .Select(e => e.Message);

                return BadRequest(errosDeValidacao);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var accessKey = result.Value.Item1;
        var resfreshKey = result.Value.Item2;

        RefreshCookieService.Set(Response, resfreshKey);

        return Ok(accessKey);
    }

    [HttpPost("autenticar")]
    public async Task<ActionResult<AccessToken>> Autenticar(AutenticarUsuarioRequest request)
    {
        var command = mapper.Map<AutenticarUsuarioCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            if (result.HasError(e => e.HasMetadata("TipoErro", m => m.Equals("RequisicaoInvalida"))))
            {
                var errosDeValidacao = result.Errors
                    .SelectMany(e => e.Reasons.OfType<IError>())
                    .Select(e => e.Message);

                return BadRequest(errosDeValidacao);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var accessKey = result.Value.Item1;
        var resfreshKey = result.Value.Item2;

        RefreshCookieService.Set(Response, resfreshKey);

        return Ok(accessKey);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Rotacionar()
    {
        var refreshToken = RefreshCookieService.Get(Request);

        if (refreshToken is null)
            return Unauthorized("O token de rotação não foi encontrado.");

        var result = await mediator.Send(new RotacionarTokenCommand(refreshToken));

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError);

        var accessKey = result.Value.Item1;
        var resfreshKey = result.Value.Item2;

        RefreshCookieService.Set(Response, resfreshKey);

        return Ok(accessKey);
    }

    [HttpPost("sair")]
    [Authorize]
    public async Task<IActionResult> Sair()
    {
        var result = await mediator.Send(new SairCommand());

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError);

        RefreshCookieService.Clear(Response);

        return NoContent();
    }
}
