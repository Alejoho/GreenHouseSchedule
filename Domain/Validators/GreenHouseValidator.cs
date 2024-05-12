using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators
{
    internal class GreenHouseValidator : AbstractValidator<GreenHouse>
    {
        public GreenHouseValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name).NotEmpty().WithName("Nombre")
                .WithMessage("El {PropertyName} no debe estar vacío.")
                .MaximumLength(50)
                .WithMessage("El {PropertyName} no debe exceder los 50 caracteres.");
            RuleFor(x => x.Width).GreaterThan(0).LessThan(200).When(x => x.Width != null)
                .WithName("Ancho")
                .WithMessage("El {PropertyName} debe estar entre 0 y 200.");
            RuleFor(x => x.Length).GreaterThan(0).LessThan(200).When(x => x.Length != null)
                .WithName("Largo")
                .WithMessage("El {PropertyName} debe estar entre 0 y 200.");
            RuleFor(x => x.SeedTrayArea).GreaterThan(0)
                .WithName("Área de bandejas")
                .WithMessage("El {PropertyName} debe ser mayor que 0.");
            RuleFor(x => x.AmountOfBlocks).Must(amountOfBlocks => amountOfBlocks > 0 && amountOfBlocks < 10)
                .WithName("Cantidad de bloques")
                .WithMessage("La {PropertyName} debe estar entre 0 y 10.");
        }
    }
}
