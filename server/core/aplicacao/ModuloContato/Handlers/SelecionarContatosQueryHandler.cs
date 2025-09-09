using AutoMapper;
using eAgenda.Core.Aplicacao.Compartilhado;
using eAgenda.Core.Aplicacao.ModuloContato.Commands;
using eAgenda.Core.Dominio.ModuloAutenticacao;
using eAgenda.Core.Dominio.ModuloContato;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace eAgenda.Core.Aplicacao.ModuloContato.Handlers;

public class SelecionarContatosQueryHandler(
    IRepositorioContato repositorioContato,
    ITenantProvider tenantProvider,
    IMapper mapper,
    IDistributedCache cache,
    ILogger<SelecionarContatosQueryHandler> logger
) : IRequestHandler<SelecionarContatosQuery, Result<SelecionarContatosResult>>
{
    public async Task<Result<SelecionarContatosResult>> Handle(
        SelecionarContatosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var cacheQuery = query.Quantidade.HasValue ? $"q={query.Quantidade.Value}" : "q=all";
            var cacheKey = $"contatos:u={tenantProvider.UsuarioId.GetValueOrDefault()}:{cacheQuery}";

            // 1) Tenta obter cache
            var jsonCache = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonCache))
            {
                var registrosEmCache = JsonSerializer.Deserialize<SelecionarContatosResult>(jsonCache);

                if (registrosEmCache is not null)
                    return Result.Ok(registrosEmCache);
            }

            // 2) Cache miss -> busca no repositório
            var registros = query.Quantidade.HasValue
                ? await repositorioContato.SelecionarRegistrosAsync(query.Quantidade.Value)
                : await repositorioContato.SelecionarRegistrosAsync();

            var resultDto = mapper.Map<SelecionarContatosResult>(registros);

            // 3) Grava no cache
            var jsonPayload = JsonSerializer.Serialize(resultDto);

            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60) };

            await cache.SetStringAsync(
                cacheKey,
                jsonPayload,
                cacheOptions,
                cancellationToken
            );

            return Result.Ok(resultDto);
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
