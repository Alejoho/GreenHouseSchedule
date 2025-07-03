using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators;

public class SpeciesValidator : AbstractValidator<Species>
{
    public SpeciesValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name).NotEmpty().WithName("Nombre")
            .WithMessage("El {PropertyName} no debe estar vacío.")
            .MaximumLength(50)
            .WithMessage("El {PropertyName} no debe exceder los 50 caracteres.");
        RuleFor(x => x.ProductionDays).NotEmpty().WithName("Días de producción")
            .WithMessage("Los {PropertyName} no debe estar vacío.")
            .Must(productionDays => productionDays > 0 && productionDays < 100)
            .WithMessage("El {PropertyName} debe estar entre 0 y 100.");
        RuleFor(x => x.WeightOf1000Seeds).GreaterThan(0).LessThan(2000)
            .When(x => x.WeightOf1000Seeds != null)
            .WithName("Peso de 1000 semillas")
            .WithMessage("El {PropertyName} debe estar entre 0 y 2000.");
        RuleFor(x => x.AmountOfSeedsPerHectare).NotEmpty()
            .WithName("Semillas en una hectárea")
            .WithMessage("Las {PropertyName} no debe estar vacío.")
            .GreaterThan(0).WithMessage("Las {PropertyName} deben ser mayor que 0.");

        // TODO: If this value is zero the validation thinks that the texblock is empty
        RuleFor(x => x.WeightOfSeedsPerHectare).NotEmpty()
            .WithName("Peso de una hectárea de semillas")
            .WithMessage("El {PropertyName} no debe estar vacío.")
            .Must(WeightOfSeedsPerHectare => WeightOfSeedsPerHectare > 0 && WeightOfSeedsPerHectare < 10000)
            .WithMessage("El {PropertyName} debe ser mayor que 0 y menor que 10000.");


    }
}
