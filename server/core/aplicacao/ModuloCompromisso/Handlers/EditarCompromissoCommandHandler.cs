using AutoMapper;
using eAgenda.Core.Aplicacao.Compartilhado;
using eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;
using eAgenda.Core.Dominio.Compartilhado;
using eAgenda.Core.Dominio.ModuloAutenticacao;
using eAgenda.Core.Dominio.ModuloCompromisso;
using eAgenda.Core.Dominio.ModuloContato;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eAgenda.Core.Aplicacao.ModuloCompromisso.Handlers;

public class EditarCompromissoCommandHandler(
    IRepositorioCompromisso repositorioCompromisso,
    IRepositorioContato repositorioContato,
    ITenantProvider tenantProvider,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IDistributedCache cache,
    IValidator<EditarCompromissoCommand> validator,
    ILogger<EditarCompromissoCommandHandler> logger
) : IRequestHandler<EditarCompromissoCommand, Result<EditarCompromissoResult>>
{
    public async Task<Result<EditarCompromissoResult>> Handle(
        EditarCompromissoCommand command, CancellationToken cancellationToken)
    {
        var resultadoValidacao = await validator.ValidateAsync(command, cancellationToken);

        if (!resultadoValidacao.IsValid)
        {
            var erros = resultadoValidacao.Errors.Select(e => e.ErrorMessage);

            var erroFormatado = ResultadosErro.RequisicaoInvalidaErro(erros);

            return Result.Fail(erroFormatado);
        }

        var registros = await repositorioCompromisso.SelecionarRegistrosAsync();

        if (registros.Any(i => !i.Id.Equals(command.Id) && i.Assunto.Equals(command.Assunto)))
            return Result.Fail(ResultadosErro.RegistroDuplicadoErro("Já existe um compromisso registrado com este assunto."));

        if (registros.Any(i =>
            !i.Id.Equals(command.Id) &&
            i.Data.Date == command.Data.Date &&
            (i.HoraInicio >= command.HoraInicio || i.HoraTermino <= command.HoraTermino)
        ))
            return Result.Fail(ResultadosErro.RegistroDuplicadoErro("Já existe um compromisso registrado com dentro deste horário."));

        try
        {
            var compromissoEditado = mapper.Map<Compromisso>(command);

            if (command.ContatoId.HasValue)
                compromissoEditado.Contato = await repositorioContato.SelecionarRegistroPorIdAsync(command.ContatoId.Value);

            await repositorioCompromisso.EditarAsync(command.Id, compromissoEditado);

            await unitOfWork.CommitAsync();

            // Invalida o cache
            var cacheKey = $"compromissos:u={tenantProvider.UsuarioId.GetValueOrDefault()}:q=all";

            await cache.RemoveAsync(cacheKey, cancellationToken);

            var result = mapper.Map<EditarCompromissoResult>(compromissoEditado);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a edição de {@Registro}.",
                command
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
