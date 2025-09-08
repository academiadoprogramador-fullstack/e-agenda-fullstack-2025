using eAgenda.Core.Aplicacao.Compartilhado;
using eAgenda.Core.Aplicacao.ModuloAutenticacao.Commands;
using eAgenda.Core.Dominio.ModuloAutenticacao;
using eAgenda.Infraestrutura.Orm.Compartilhado;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace eAgenda.Core.Aplicacao.ModuloAutenticacao.Handlers;

public class SairCommandHandler(
    AppDbContext dbContext,
    UserManager<Usuario> userManager,
    SignInManager<Usuario> signInManager,
    ITenantProvider tenantProvider,
    IRefreshTokenProvider refreshTokenProvider
) : IRequestHandler<SairCommand, Result>
{
    public async Task<Result> Handle(SairCommand command, CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();

        var usuarioId = tenantProvider.UsuarioId.GetValueOrDefault();

        await refreshTokenProvider.RevogarTokensUsuarioAsync(usuarioId, "Logout");

        var usuarioEncontrado = await userManager.FindByIdAsync(usuarioId.ToString());

        if (usuarioEncontrado is null)
            return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro("Não foi possível encontrar o usuário requisitado."));

        usuarioEncontrado.AccessTokenVersion++;

        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }
}