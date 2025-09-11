using eAgenda.Core.Dominio.ModuloCompromisso;
using FluentResults;
using MediatR;
using System.Collections.Immutable;

namespace eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;

public record SelecionarCompromissosQuery(int? Quantidade)
    : IRequest<Result<SelecionarCompromissosResult>>;

public record SelecionarCompromissosResult(ImmutableList<SelecionarCompromissosDto> Compromissos);

public record SelecionarCompromissosDto(
    Guid Id,
    string Assunto,
    DateTime Data,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    string? Contato
);