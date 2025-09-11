using eAgenda.Core.Dominio.ModuloCompromisso;

namespace eAgenda.WebApi.Models.ModuloCompromisso;

public record CadastrarCompromissoRequest(
    string Assunto,
    DateTime Data,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId = null
);

public record CadastrarCompromissoResponse(Guid Id);