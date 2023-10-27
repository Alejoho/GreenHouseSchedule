using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators
{
    internal class SeedTrayValidator : AbstractValidator<SeedTray>
    {
        public SeedTrayValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            
            RuleFor(x => x.Name).NotEmpty().WithName("Nombre")
                .WithMessage("El {PropertyName} no debe estar vacío.")
                .MaximumLength(50)
                .WithMessage("El {PropertyName} no debe exceder los 50 caracteres.");
            RuleFor(x => x.TotalAlveolus).NotEmpty().WithName("Total de alvéolos")
                .WithMessage("El {PropertyName} no debe estar vacío.")
                .Must(totalAlveolus => totalAlveolus > 0 && totalAlveolus < 400)
                .WithMessage("El {PropertyName} debe estar entre 0 y 400.");
            RuleFor(x => x.AlveolusLength)
                .Must(alveolusLength => alveolusLength > 0 && alveolusLength < 50)
                .When(x => x.AlveolusLength != null).WithName("Alvéolos a lo largo")
                .WithMessage("El {PropertyName} debe estar entre 0 y 50.");
            //NEXT terminar el seedtray validator


        }
    }
}
