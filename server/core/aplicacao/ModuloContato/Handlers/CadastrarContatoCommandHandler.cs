using AutoMapper;
using eAgenda.Core.Aplicacao.Compartilhado;
using eAgenda.Core.Aplicacao.ModuloContato.Commands;
using eAgenda.Core.Dominio.Compartilhado;
using eAgenda.Core.Dominio.ModuloAutenticacao;
using eAgenda.Core.Dominio.ModuloContato;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eAgenda.Core.Aplicacao.ModuloContato.Handlers;

public class CadastrarContatoCommandHandler(
    IRepositorioContato repositorioContato,
    IUnitOfWork unitOfWork,
    ITenantProvider tenantProvider,
    IMapper mapper,
    IDistributedCache cache,
    IValidator<CadastrarContatoCommand> validator,
    ILogger<CadastrarContatoCommandHandler> logger
) : IRequestHandler<CadastrarContatoCommand, Result<CadastrarContatoResult>>
{
    public async Task<Result<CadastrarContatoResult>> Handle(
        CadastrarContatoCommand command, CancellationToken cancellationToken)
    {
        var resultadoValidacao = await validator.ValidateAsync(command, cancellationToken);

        if (!resultadoValidacao.IsValid)
        {
            var erros = resultadoValidacao.Errors.Select(e => e.ErrorMessage);

            var erroFormatado = ResultadosErro.RequisicaoInvalidaErro(erros);

            return Result.Fail(erroFormatado);
        }

        var registros = await repositorioContato.SelecionarRegistrosAsync();

        if (registros.Any(i => i.Nome.Equals(command.Nome)))
            return Result.Fail(ResultadosErro.RegistroDuplicadoErro("Já existe um contato registrado com este nome."));

        try
        {
            var contato = mapper.Map<Contato>(command);

            contato.UsuarioId = tenantProvider.UsuarioId.GetValueOrDefault();

            await repositorioContato.CadastrarAsync(contato);

            await unitOfWork.CommitAsync();

            // Invalida o cache
            await cache.RemoveAsync($"contatos:u={contato.UsuarioId}:q=all", cancellationToken);

            var result = mapper.Map<CadastrarContatoResult>(contato);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o registro de {@Registro}.",
                command
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
