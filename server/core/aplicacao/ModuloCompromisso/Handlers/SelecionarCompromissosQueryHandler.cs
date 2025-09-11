using AutoMapper;
using eAgenda.Core.Aplicacao.Compartilhado;
using eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;
using eAgenda.Core.Dominio.ModuloAutenticacao;
using eAgenda.Core.Dominio.ModuloCompromisso;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace eAgenda.Core.Aplicacao.ModuloCompromisso.Handlers;

public class SelecionarCompromissosQueryHandler(
    IRepositorioCompromisso repositorioCompromisso,
    ITenantProvider tenantProvider,
    IMapper mapper,
    IDistributedCache cache,
    ILogger<SelecionarCompromissosQueryHandler> logger
) : IRequestHandler<SelecionarCompromissosQuery, Result<SelecionarCompromissosResult>>
{
    public async Task<Result<SelecionarCompromissosResult>> Handle(
        SelecionarCompromissosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var cacheQuery = query.Quantidade.HasValue ? $"q={query.Quantidade.Value}" : "q=all";
            var cacheKey = $"contatos:u={tenantProvider.UsuarioId.GetValueOrDefault()}:{cacheQuery}";

            // 1) Tenta acessar o cache
            var jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                var resultadoEmCache = JsonSerializer.Deserialize<SelecionarCompromissosResult>(jsonString);

                if (resultadoEmCache is not null)
                    return Result.Ok(resultadoEmCache);
            }

            // 2) Cache miss -> busca no repositório
            var registros = query.Quantidade.HasValue ?
                await repositorioCompromisso.SelecionarRegistrosAsync(query.Quantidade.Value) :
                await repositorioCompromisso.SelecionarRegistrosAsync();

            var result = mapper.Map<SelecionarCompromissosResult>(registros);

            // 3) Salva os resultados novos no cache
            var jsonPayload = JsonSerializer.Serialize(result);

            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60) };

            await cache.SetStringAsync(cacheKey, jsonPayload, cacheOptions, cancellationToken);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção de {@Registros}.",
                query
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
