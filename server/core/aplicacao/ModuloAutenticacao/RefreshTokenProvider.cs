using eAgenda.Core.Dominio.ModuloAutenticacao;
using eAgenda.Infraestrutura.Orm.Compartilhado;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace eAgenda.Core.Aplicacao.ModuloAutenticacao;

public class RefreshTokenProvider(
    AppDbContext dbContext,
    UserManager<Usuario> userManager,
    IHttpContextAccessor contextAccessor
) : IRefreshTokenProvider
{
    private readonly HttpContext? httpContext = contextAccessor.HttpContext;
    private readonly int expiracaoTokenEmDias = 7;

    public async Task<RefreshToken> GerarRefreshTokenAsync(Usuario usuario)
    {
        var tokenString = GerarTokenOpaco();

        var hash = Hash(tokenString);

        var now = DateTime.UtcNow;

        var novoRefreshToken = new RefreshToken
        {
            UsuarioId = usuario.Id,
            TokenHash = hash,
            CriadoEmUtc = now,
            ExpiraEmUtc = now.AddDays(expiracaoTokenEmDias),
            IpCriacao = httpContext?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString()
        };

        dbContext.RefreshTokens.Add(novoRefreshToken);

        await dbContext.SaveChangesAsync();

        return novoRefreshToken;
    }

    public async Task<(Usuario usuario, RefreshToken novoRefreshToken)> RotacionarRefreshTokenAsync(string refreshTokenHash)
    {
        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == refreshTokenHash);

        if (token is null)
            throw new SecurityTokenException("Refresh token inválido.");

        if (token.RevogadoEmUtc is not null)
            throw new SecurityTokenException("Refresh token já utilizado/revogado (suspeita de reuso).");

        if (token.ExpiraEmUtc <= DateTime.UtcNow)
            throw new SecurityTokenException("Refresh token expirado.");

        // carrega usuário
        var usuario = await userManager.FindByIdAsync(token.UsuarioId.ToString())
            ?? throw new SecurityTokenException("Usuário não encontrado.");

        // cria novo token e revoga o antigo
        var novoToken = GerarTokenOpaco();

        var novoHash = Hash(novoToken);

        token.RevogadoEmUtc = DateTime.UtcNow;
        token.SubstituidoPorTokenHash = novoHash;
        token.MotivoRevogacao = "Rotação";

        var now = DateTime.UtcNow;

        var novoRefreshToken = new RefreshToken
        {
            UsuarioId = usuario.Id,
            TokenHash = novoHash,
            CriadoEmUtc = now,
            ExpiraEmUtc = now.AddDays(expiracaoTokenEmDias),
            IpCriacao = httpContext?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString()
        };

        await dbContext.RefreshTokens.AddAsync(novoRefreshToken);

        await dbContext.SaveChangesAsync();

        return (usuario, novoRefreshToken);
    }

    public async Task RevogarTokensUsuarioAsync(Guid usuarioId, string motivo)
    {
        var tokensAtivos = await dbContext.RefreshTokens
            .Where(t => t.UsuarioId == usuarioId && t.RevogadoEmUtc == null && t.ExpiraEmUtc > DateTime.UtcNow)
            .ToListAsync();

        foreach (var t in tokensAtivos)
        {
            t.RevogadoEmUtc = DateTime.UtcNow;
            t.MotivoRevogacao = motivo;
        }

        await dbContext.SaveChangesAsync();
    }

    private static string Hash(string raw)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));

        return Convert.ToBase64String(bytes);
    }

    private static string GerarTokenOpaco()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }
}