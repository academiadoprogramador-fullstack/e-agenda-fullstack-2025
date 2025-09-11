using eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;
using System.Collections.Immutable;

namespace eAgenda.WebApi.Models.ModuloCompromisso;

public record SelecionarCompromissosRequest(int? Quantidade);

public record SelecionarCompromissosResponse(
    int Quantidade,
    ImmutableList<SelecionarCompromissosDto> Compromissos
);