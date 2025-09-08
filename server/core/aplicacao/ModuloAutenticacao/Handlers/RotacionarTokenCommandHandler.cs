using eAgenda.Core.Aplicacao.ModuloAutenticacao.Commands;
using eAgenda.Core.Dominio.ModuloAutenticacao;
using FluentResults;
using MediatR;

namespace eAgenda.Core.Aplicacao.ModuloAutenticacao.Handlers;

public class RotacionarTokenCommandHandler(
    IAccessTokenProvider accessTokenProvider,
    IRefreshTokenProvider refreshTokenProvider
) : IRequestHandler<RotacionarTokenCommand, Result<(AccessToken, RefreshToken)>>
{
    public async Task<Result<(AccessToken, RefreshToken)>> Handle(RotacionarTokenCommand command, CancellationToken cancellationToken)
    {
        var (usuario, novoRefreshToken) = await refreshTokenProvider.RotacionarRefreshTokenAsync(command.RefreshTokenString);

        var novoAccessToken = accessTokenProvider.GerarAccessToken(usuario);

        return Result.Ok((novoAccessToken, novoRefreshToken));
    }
}