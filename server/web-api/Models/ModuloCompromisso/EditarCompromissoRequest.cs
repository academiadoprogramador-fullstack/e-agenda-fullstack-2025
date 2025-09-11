using eAgenda.Core.Dominio.ModuloCompromisso;

namespace eAgenda.WebApi.Models.ModuloCompromisso;

public record EditarCompromissoRequest(
    string Assunto,
    DateTime Data,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId = null
);

public record EditarCompromissoResponse(
    string Assunto,
    DateTime Data,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    string? Contato
);