using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators;

public class OrganizationValidator : AbstractValidator<Organization>
{
    public OrganizationValidator()
    {
        //NEXT - do the ctor of the validator of organizations

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name).NotEmpty().WithName("Nombre")
            .WithMessage("El {PropertyName} no debe estar vacío.")
            .MaximumLength(50)
            .WithMessage("El {PropertyName} no debe exceder los 50 caracteres.");
        RuleFor(x => x.TypeOfOrganizationId).GreaterThan((byte)0).WithName("Tipo")
            .WithMessage("El {PropertyName} no debe estar vacío.");
        RuleFor(x => x.MunicipalityId).GreaterThan((short)0).WithName("Locación")
            .WithMessage("La {PropertyName} no debe estar vacía.");
    }
}
