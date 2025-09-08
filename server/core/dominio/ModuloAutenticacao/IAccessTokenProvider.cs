namespace eAgenda.Core.Dominio.ModuloAutenticacao;

public interface IAccessTokenProvider
{
    AccessToken GerarAccessToken(Usuario usuario);
}

public record AccessToken(
    string Chave,
    DateTime Expiracao,
    UsuarioAutenticado UsuarioAutenticado
);