using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.SpecieId).GreaterThan((byte)0).WithName("Species")
            .WithMessage("La {PropertyName} no debe estar vacía.");

        RuleFor(x => x.Variety).NotEmpty().WithName("Nombre")
            .WithMessage("El {PropertyName} no debe estar vacío.")
            .MaximumLength(50)
            .WithMessage("El {PropertyName} no debe exceder los 50 caracteres.");
    }
}
