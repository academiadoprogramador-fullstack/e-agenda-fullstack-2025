﻿using eAgenda.Core.Aplicacao.Compartilhado;
using eAgenda.Core.Aplicacao.ModuloAutenticacao.Commands;
using eAgenda.Core.Dominio.ModuloAutenticacao;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace eAgenda.Core.Aplicacao.ModuloAutenticacao.Handlers;

public class RegistrarUsuarioCommandHandler(
    UserManager<Usuario> userManager,
    IAccessTokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider
) : IRequestHandler<RegistrarUsuarioCommand, Result<(AccessToken, RefreshToken)>>
{
    public async Task<Result<(AccessToken, RefreshToken)>> Handle(RegistrarUsuarioCommand command, CancellationToken cancellationToken)
    {
        if (!command.Senha.Equals(command.ConfirmarSenha))
            return Result.Fail(ResultadosErro.RequisicaoInvalidaErro("A confirmação de senha falhou."));

        var usuario = new Usuario
        {
            FullName = command.NomeCompleto,
            UserName = command.Email,
            Email = command.Email
        };

        var usuarioResult = await userManager.CreateAsync(usuario, command.Senha);

        if (!usuarioResult.Succeeded)
        {
            var erros = usuarioResult.Errors.Select(err =>
            {
                return err.Code switch
                {
                    "DuplicateUserName" => "Já existe um usuário com esse nome.",
                    "DuplicateEmail" => "Já existe um usuário com esse e-mail.",
                    "PasswordTooShort" => "A senha é muito curta.",
                    "PasswordRequiresNonAlphanumeric" => "A senha deve conter pelo menos um caractere especial.",
                    "PasswordRequiresDigit" => "A senha deve conter pelo menos um número.",
                    "PasswordRequiresUpper" => "A senha deve conter pelo menos uma letra maiúscula.",
                    "PasswordRequiresLower" => "A senha deve conter pelo menos uma letra minúscula.",
                    _ => err.Description
                };
            });

            return Result.Fail(ResultadosErro.RequisicaoInvalidaErro(erros));
        }

        var accessToken = tokenProvider.GerarAccessToken(usuario);

        if (accessToken is null)
            return Result.Fail(ResultadosErro.ExcecaoInternaErro(new Exception("Falha ao gerar token de acesso.")));

        var refreshToken = await refreshTokenProvider.GerarRefreshTokenAsync(usuario);

        if (accessToken is null)
            return Result.Fail(ResultadosErro.ExcecaoInternaErro(new Exception("Falha ao gerar token de rotação.")));

        return Result.Ok((accessToken, refreshToken));
    }
}
