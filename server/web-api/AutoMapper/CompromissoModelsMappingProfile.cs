using AutoMapper;
using eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;
using eAgenda.Core.Aplicacao.ModuloContato.Commands;
using eAgenda.Core.Dominio.ModuloCompromisso;
using eAgenda.Core.Dominio.ModuloContato;
using eAgenda.WebApi.Models.ModuloCompromisso;
using eAgenda.WebApi.Models.ModuloContato;
using System.Collections.Immutable;

namespace eAgenda.WebApi.AutoMapper;

public class CompromissoModelsMappingProfile : Profile
{
    public CompromissoModelsMappingProfile()
    {
        CreateMap<CadastrarCompromissoRequest, CadastrarCompromissoCommand>();
        CreateMap<CadastrarCompromissoResult, CadastrarCompromissoResponse>();

        CreateMap<(Guid, EditarCompromissoRequest), EditarCompromissoCommand>()
            .ConvertUsing(src => new EditarCompromissoCommand(
                src.Item1,
                src.Item2.Assunto,
                src.Item2.Data,
                src.Item2.HoraInicio,
                src.Item2.HoraTermino,
                src.Item2.Tipo,
                src.Item2.Local,
                src.Item2.Link,
                src.Item2.ContatoId
            ));

        CreateMap<EditarCompromissoResult, EditarCompromissoResponse>()
            .ConvertUsing(src => new EditarCompromissoResponse(
                src.Assunto,
                src.Data,
                src.HoraInicio,
                src.HoraTermino,
                src.Tipo,
                src.Local,
                src.Link,
                src.Contato
            ));

        CreateMap<Guid, ExcluirCompromissoCommand>()
          .ConstructUsing(src => new ExcluirCompromissoCommand(src));

        CreateMap<Guid, SelecionarCompromissoPorIdQuery>()
            .ConvertUsing(src => new SelecionarCompromissoPorIdQuery(src));

        CreateMap<SelecionarCompromissoPorIdResult, SelecionarCompromissoPorIdResponse>();

        CreateMap<SelecionarCompromissosRequest, SelecionarCompromissosQuery>();

        CreateMap<SelecionarCompromissosResult, SelecionarCompromissosResponse>()
            .ConvertUsing((src, dest, ctx) => new SelecionarCompromissosResponse(
                src.Compromissos.Count,
                src?.Compromissos.Select(c => ctx.Mapper.Map<SelecionarCompromissosDto>(c)).ToImmutableList() ?? ImmutableList<SelecionarCompromissosDto>.Empty
            ));
    }
}
