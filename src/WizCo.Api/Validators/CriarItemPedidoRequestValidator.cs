namespace WizCo.Api.Validators;

using FluentValidation;
using WizCo.Api.DTOs.Requests;

public sealed class CriarItemPedidoRequestValidator : AbstractValidator<CriarItemPedidoRequest>
{
    public CriarItemPedidoRequestValidator()
    {
        RuleFor(x => x.ProdutoNome)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(200).WithMessage("O nome do produto deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

        RuleFor(x => x.PrecoUnitario)
            .GreaterThan(0).WithMessage("O preço unitário deve ser maior que zero.");
    }
}
