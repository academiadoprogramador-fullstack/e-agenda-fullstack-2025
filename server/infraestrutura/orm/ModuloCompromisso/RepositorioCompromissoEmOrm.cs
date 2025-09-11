using eAgenda.Core.Dominio.ModuloCompromisso;
using eAgenda.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.Orm.ModuloCompromisso;

public class RepositorioCompromissoEmOrm(AppDbContext contexto)
    : RepositorioBaseEmOrm<Compromisso>(contexto), IRepositorioCompromisso
{
    public override async Task<List<Compromisso>> SelecionarRegistrosAsync()
    {
        return await registros.Include(x => x.Contato).ToListAsync();
    }

    public override async Task<Compromisso?> SelecionarRegistroPorIdAsync(Guid idRegistro)
    {
        return await registros.Include(x => x.Contato).FirstOrDefaultAsync(x => x.Id == idRegistro);
    }
}