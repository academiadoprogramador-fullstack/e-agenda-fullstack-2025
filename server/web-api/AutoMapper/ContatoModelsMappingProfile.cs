using AutoMapper;
using eAgenda.Core.Aplicacao.ModuloContato.Commands;
using eAgenda.WebApi.Models.ModuloContato;
using System.Collections.Immutable;

namespace eAgenda.WebApi.AutoMapper;

public class ContatoModelsMappingProfile : Profile
{
    public ContatoModelsMappingProfile()
    {
        CreateMap<CadastrarContatoRequest, CadastrarContatoCommand>();
        CreateMap<CadastrarContatoResult, CadastrarContatoResponse>();

        CreateMap<(Guid, EditarContatoRequest), EditarContatoCommand>()
            .ConvertUsing(src => new EditarContatoCommand(
                src.Item1,
                src.Item2.Nome,
                src.Item2.Telefone,
                src.Item2.Email,
                src.Item2.Empresa,
                src.Item2.Cargo
            ));

        CreateMap<EditarContatoResult, EditarContatoResponse>();

        CreateMap<SelecionarContatosRequest, SelecionarContatosQuery>();

        CreateMap<SelecionarContatosResult, SelecionarContatosResponse>()
            .ConvertUsing((src, dest, ctx) => new SelecionarContatosResponse(
                src.Contatos.Count,
                src?.Contatos.Select(c => ctx.Mapper.Map<SelecionarContatosDto>(c)).ToImmutableList() ?? ImmutableList<SelecionarContatosDto>.Empty
            ));

        CreateMap<Guid, SelecionarContatoPorIdQuery>();
        CreateMap<SelecionarContatoPorIdResult, SelecionarContatoPorIdResponse>();
    }
}
