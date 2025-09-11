using AutoMapper;
using eAgenda.Core.Aplicacao.Compartilhado;
using eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;
using eAgenda.Core.Dominio.ModuloCompromisso;
using eAgenda.Core.Dominio.ModuloContato;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eAgenda.Core.Aplicacao.ModuloCompromisso.Handlers;

public class SelecionarCompromissoPorIdQueryHandler(
    IRepositorioCompromisso repositorioCompromisso,
    IMapper mapper,
    ILogger<SelecionarCompromissoPorIdQueryHandler> logger
) : IRequestHandler<SelecionarCompromissoPorIdQuery, Result<SelecionarCompromissoPorIdResult>>
{
    public async Task<Result<SelecionarCompromissoPorIdResult>> Handle(
        SelecionarCompromissoPorIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var registro = await repositorioCompromisso.SelecionarRegistroPorIdAsync(query.Id);

            if (registro is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(query.Id));

            var result = mapper.Map<SelecionarCompromissoPorIdResult>(registro);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção de {@Registro}.",
                query
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
