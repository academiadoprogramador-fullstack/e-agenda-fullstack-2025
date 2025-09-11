using FluentResults;
using MediatR;

namespace eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;

public record ExcluirCompromissoCommand(Guid Id) : IRequest<Result<ExcluirCompromissoResult>>;

public record ExcluirCompromissoResult();