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
                .WithMessage("El {PropertyName} debe estar entre 0 y 400.")
                .Equal(x => (short)(x.AlveolusLength * x.AlveolusWidth))
                .When(x => x.AlveolusLength != null && x.AlveolusWidth != null)
                .WithMessage("El {PropertyName} no se corresponde con largo y ancho de alvéolos.");
            RuleFor(x => x.AlveolusLength)
                .Must(alveolusLength => alveolusLength > 0 && alveolusLength < 50)
                .When(x => x.AlveolusLength != null).WithName("Alvéolos a lo largo")
                .WithMessage("El {PropertyName} debe estar entre 0 y 50.");
            RuleFor(x => x.AlveolusWidth)
                .Must(alveolusWidth => alveolusWidth > 0 && alveolusWidth < 50)
                .When(x => x.AlveolusWidth != null).WithName("Alvéolos a lo ancho")
                .WithMessage("El {PropertyName} debe estar entre 0 y 50.");
            RuleFor(x => x.TrayLength)
                .Must(trayLength => trayLength > 0 && trayLength < 1.5m)
                .When(x => x.TrayLength != null).WithName("Largo de la bandeja")
                .WithMessage("El {PropertyName} debe estar entre 0 y 1.5.");
            RuleFor(x => x.TrayWidth)
                .Must(trayWidth => trayWidth > 0 && trayWidth < 1.5m)
                .When(x => x.TrayWidth != null).WithName("Ancho de la bandeja")
                .WithMessage("El {PropertyName} debe estar entre 0 y 1.5.");
            RuleFor(x => x.TrayArea)
                .Must(trayArea => trayArea > 0 && trayArea < 2.25m)
                .When(x => x.TrayArea != null).WithName("Área de la bandejas")
                .WithMessage("El {PropertyName} debe estar entre 0 y 2.25.");
            RuleFor(x => x.LogicalTrayArea).NotEmpty().WithName("Área lógica de la bandeja")
                .WithMessage("El {PropertyName} no debe estar vacío ni contener el valor 0.")
                .GreaterThanOrEqualTo(x => x.TrayArea)
                .WithMessage("El {PropertyName} debe ser mayor o igual que el " +
                "área de la bandeja \n(Largo de la bandeja * Ancho de la bandeja).")
                .LessThan(4).WithMessage("El {PropertyName} debe ser menor que 4");
            RuleFor(x => x.TotalAmount).NotEmpty().WithName("Cantidad de bandejas")
                .WithMessage("El {PropertyName} no debe estar vacío ni contener el valor 0.")
                .Must(totalAmount => totalAmount > 0 && totalAmount < 10000)
                .WithMessage("El {PropertyName} debe estar entre 0 y 10000.");
            RuleFor(x => x.Material).MaximumLength(20).WithName("Material")
                .When(x => x.Material != null)
                .WithMessage("El {PropertyName} no debe exceder los 20 caracteres.");
        }
    }
}
