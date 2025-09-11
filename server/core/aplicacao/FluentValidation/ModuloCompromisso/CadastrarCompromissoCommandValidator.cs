using eAgenda.Core.Aplicacao.ModuloCompromisso.Commands;
using eAgenda.Core.Dominio.ModuloCompromisso;
using FluentValidation;

namespace eAgenda.Core.Aplicacao.FluentValidation.ModuloCompromisso;

public class CadastrarCompromissoCommandValidator : AbstractValidator<CadastrarCompromissoCommand>
{
    public CadastrarCompromissoCommandValidator()
    {
        RuleFor(x => x.Assunto)
           .NotEmpty().WithMessage("O assunto é obrigatório.")
           .MinimumLength(2).WithMessage("O assunto deve ter pelo menos {MinLength} caracteres.")
           .MaximumLength(100).WithMessage("O assunto deve conter no máximo {MaxLength} caracteres.");

        When(x => x.Tipo == TipoCompromisso.Remoto, () =>
        {
            RuleFor(x => x.Link)
               .NotNull()
               .NotEmpty();

        }).Otherwise(() =>
        {
            RuleFor(x => x.Local)
                .NotNull()
                .NotEmpty();
        });
    }
}