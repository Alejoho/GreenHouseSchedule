using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators
{
    internal class GreenHouseValidator : AbstractValidator<GreenHouse>
    {
        public GreenHouseValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Width).GreaterThan(0).LessThan(200).When(x => x.Width != null)
                .WithName("Ancho")
                .WithMessage("{PropertyName} inválido. El {PropertyName} debe estar entre 0 y 200.");
            RuleFor(x => x.Length).GreaterThan(0).LessThan(200).When(x => x.Length != null)
                .WithName("Largo")
                .WithMessage("{PropertyName} inválido. El {PropertyName} debe estar entre 0 y 200.");
            RuleFor(x => x.SeedTrayArea).GreaterThan(0)
                .WithName("Área de bandejas")
                .WithMessage("{PropertyName} inválido. El {PropertyName} debe ser mayor que 0.");
            RuleFor(x => x.AmountOfBlocks).Must(AmountOfBlocks => AmountOfBlocks > 0 && AmountOfBlocks < 10)
                .WithName("Cantidad de bloques")
                .WithMessage("{PropertyName} inválido. La {PropertyName} debe estar entre 0 y 10.");
        }
    }
}
