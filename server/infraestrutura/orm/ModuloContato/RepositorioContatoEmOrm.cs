using eAgenda.Core.Dominio.ModuloContato;
using eAgenda.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.Orm.ModuloContato;

public class RepositorioContatoEmOrm(AppDbContext contexto) : RepositorioBaseEmOrm<Contato>(contexto), IRepositorioContato
{
    public override async Task<Contato?> SelecionarRegistroPorIdAsync(Guid idRegistro)
    {
        return await registros
            .Include(c => c.Compromissos)
            .FirstOrDefaultAsync(x => x.Id == idRegistro);
    }
}
