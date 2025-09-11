using eAgenda.Core.Aplicacao.Compartilhado;
using eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;
using eAgenda.Core.Dominio.Compartilhado;
using eAgenda.Core.Dominio.ModuloAutenticacao;
using eAgenda.Core.Dominio.ModuloCompromisso;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eAgenda.Core.Aplicacao.ModuloCompromisso.Handlers;

public class ExcluirCompromissoCommandHandler(
    IRepositorioCompromisso repositorioCompromisso,
    ITenantProvider tenantProvider,
    IUnitOfWork unitOfWork,
    IDistributedCache cache,
    ILogger<ExcluirCompromissoCommandHandler> logger
) : IRequestHandler<ExcluirCompromissoCommand, Result<ExcluirCompromissoResult>>
{
    public async Task<Result<ExcluirCompromissoResult>> Handle(
        ExcluirCompromissoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await repositorioCompromisso.ExcluirAsync(command.Id);

            await unitOfWork.CommitAsync();

            var cacheKey = $"compromissos:u={tenantProvider.UsuarioId.GetValueOrDefault()}:q=all";

            await cache.RemoveAsync(cacheKey, cancellationToken);

            var result = new ExcluirCompromissoResult();

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a exclusão de {@Registro}.",
                command
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
