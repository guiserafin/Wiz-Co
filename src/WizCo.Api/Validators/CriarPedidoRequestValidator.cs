namespace WizCo.Api.Validators;

using FluentValidation;
using WizCo.Api.DTOs.Requests;

public sealed class CriarPedidoRequestValidator : AbstractValidator<CriarPedidoRequest>
{
    public CriarPedidoRequestValidator()
    {
        RuleFor(x => x.ClienteNome)
            .NotEmpty().WithMessage("O nome do cliente é obrigatório.")
            .MaximumLength(200).WithMessage("O nome do cliente deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("O pedido deve conter pelo menos um item.");

        RuleForEach(x => x.Itens)
            .SetValidator(new CriarItemPedidoRequestValidator());
    }
}
