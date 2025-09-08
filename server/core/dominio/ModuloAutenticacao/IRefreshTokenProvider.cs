namespace eAgenda.Core.Dominio.ModuloAutenticacao;

public interface IRefreshTokenProvider
{
    Task<RefreshToken> GerarRefreshTokenAsync(Usuario usuario);
    Task<(Usuario usuario, RefreshToken novoRefreshToken)> RotacionarRefreshTokenAsync(string refreshToken);
    Task RevogarTokensUsuarioAsync(Guid usuarioId, string motivo);
}

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UsuarioId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiraEmUtc { get; set; }

    public DateTime? RevogadoEmUtc { get; set; }
    public string? SubstituidoPorTokenHash { get; set; }
    public string? MotivoRevogacao { get; set; }

    public string? IpCriacao { get; set; }
    public string? UserAgent { get; set; }
}