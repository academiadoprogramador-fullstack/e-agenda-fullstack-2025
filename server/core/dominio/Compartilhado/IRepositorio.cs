namespace eAgenda.Core.Dominio.Compartilhado;

public interface IRepositorio<T> where T : EntidadeBase<T>
{
    public Task CadastrarAsync(T novoRegistro);

    public Task<bool> EditarAsync(Guid idRegistro, T registroEditado);

    public Task<bool> ExcluirAsync(Guid idRegistro);

    public Task<List<T>> SelecionarRegistrosAsync();

    public Task<List<T>> SelecionarRegistrosAsync(int quantidade);

    public Task<T?> SelecionarRegistroPorIdAsync(Guid idRegistro);
}
