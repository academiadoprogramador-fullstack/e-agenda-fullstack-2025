using eAgenda.Core.Dominio.ModuloAutenticacao;
using FluentResults;
using MediatR;

namespace eAgenda.Core.Aplicacao.ModuloAutenticacao.Commands;

public record RotacionarTokenCommand(string RefreshTokenString) : IRequest<Result<(AccessToken, RefreshToken)>>;
