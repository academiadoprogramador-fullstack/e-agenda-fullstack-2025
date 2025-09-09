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
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    public async Task<Result<SelecionarContatosResult>> Handle(SelecionarContatosQuery query, CancellationToken ct)
    {
        try
        {
            var tenantId = tenantProvider.UsuarioId.GetValueOrDefault();

            var queryCache = query.Quantidade.HasValue ? $"q={query.Quantidade.Value}" : "q=all";
            var chaveCache = $"contatos:u:{tenantId}:{queryCache}";

            // 1) Tenta cache
            var jsonEmCache = await cache.GetStringAsync(chaveCache, ct);

            if (!string.IsNullOrWhiteSpace(jsonEmCache))
            {
                var registrosEmCache = JsonSerializer.Deserialize<SelecionarContatosResult>(jsonEmCache);

                if (registrosEmCache is not null)
                    return Result.Ok(registrosEmCache);
            }

            // 2) Cache miss -> busca no repositório
            var registros = query.Quantidade.HasValue
                ? await repositorioContato.SelecionarRegistrosAsync(query.Quantidade.Value)
                : await repositorioContato.SelecionarRegistrosAsync();

            var resultDto = mapper.Map<SelecionarContatosResult>(registros);

            // 3) Grava no cache
            var payload = JsonSerializer.Serialize(resultDto);

            await cache.SetStringAsync(chaveCache, payload, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheTtl
            }, ct);

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
