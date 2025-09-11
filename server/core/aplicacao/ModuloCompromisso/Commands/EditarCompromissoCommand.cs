using eAgenda.Core.Dominio.ModuloCompromisso;
using FluentResults;
using MediatR;

namespace eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;

public record EditarCompromissoCommand(
    Guid Id,
    string Assunto,
    DateTime Data,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId = null
) : IRequest<Result<EditarCompromissoResult>>;

public record EditarCompromissoResult(
    string Assunto,
    DateTime Data,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    string? Contato
);