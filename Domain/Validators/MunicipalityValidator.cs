using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators
{
    public class MunicipalityValidator : AbstractValidator<Municipality>
    {
        public MunicipalityValidator()
        {
            //CHECK - do the ctor of the validator of municipalities
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name).NotEmpty().WithName("Nombre")
                .WithMessage("El {PropertyName} no debe estar vacío.")
                .MaximumLength(50)
                .WithMessage("El {PropertyName} no debe exceder los 50 caracteres.");
        }
    }
}
