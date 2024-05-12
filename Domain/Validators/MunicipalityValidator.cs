using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators;

public class MunicipalityValidator : AbstractValidator<Municipality>
{
    public MunicipalityValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name).NotEmpty().WithName("Nombre")
            .WithMessage("El {PropertyName} no debe estar vacío.")
            .MaximumLength(50)
            .WithMessage("El {PropertyName} no debe exceder los 50 caracteres.");
        RuleFor(x => x.ProvinceId).GreaterThan((byte)0).WithName("Provincia")
            .WithMessage("La {PropertyName} no debe estar vacía.");
    }
}
