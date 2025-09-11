using AutoMapper;
using eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;
using eAgenda.Core.Dominio.ModuloCompromisso;
using System.Collections.Immutable;

namespace eAgenda.Core.Aplicacao.AutoMapper;

public class CompromissoMappingProfile : Profile
{
    public CompromissoMappingProfile()
    {
        CreateMap<CadastrarCompromissoCommand, Compromisso>();
        CreateMap<Compromisso, CadastrarCompromissoResult>();

        CreateMap<EditarCompromissoCommand, Compromisso>();
        CreateMap<Compromisso, EditarCompromissoResult>();

        CreateMap<Compromisso, SelecionarCompromissoPorIdResult>()
         .ConvertUsing(src => new SelecionarCompromissoPorIdResult(
             src.Id,
             src.Assunto,
             src.Data,
             src.HoraInicio,
             src.HoraTermino,
             src.Tipo,
             src.Local,
             src.Link,
             src.Contato.Nome 
         ));

        CreateMap<Compromisso, SelecionarCompromissosDto>()
            .ForMember(
                dest => dest.Contato,
                opt => opt.MapFrom(src => src.Contato.Nome ?? null)
            );

        CreateMap<IEnumerable<Compromisso>, SelecionarCompromissosResult>()
         .ConvertUsing((src, dest, ctx) =>
             new SelecionarCompromissosResult(
                 src?.Select(c => ctx.Mapper.Map<SelecionarCompromissosDto>(c)).ToImmutableList() ?? ImmutableList<SelecionarCompromissosDto>.Empty
             )
         );
    }
}