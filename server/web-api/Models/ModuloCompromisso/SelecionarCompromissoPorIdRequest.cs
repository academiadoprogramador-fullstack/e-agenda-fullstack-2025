using eAgenda.Core.Dominio.ModuloCompromisso;

namespace eAgenda.WebApi.Models.ModuloCompromisso;

public record SelecionarCompromissoPorIdRequest(Guid Id);

public record SelecionarCompromissoPorIdResponse(
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