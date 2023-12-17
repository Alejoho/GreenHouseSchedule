using FluentValidation;

namespace Domain.Validators
{
    public class ConfigurationValidator :AbstractValidator<Configurations>
    {
        public ConfigurationValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            //Later - Colocate placeholders in the message of the validator instead of using hard strings.
            RuleFor(x => x.RegressionDays).InclusiveBetween(90, 180)
                .WithName("Días de retroceso")
                .WithMessage("La configuración {PropertyName} debe estar entre {From} y {To}.");
            RuleFor(x => x.DailySowingPotential).LessThanOrEqualTo(1500)
                .WithName("Potencial de siembra diario")
                .WithMessage("La configuración {PropertyName} no debe ser mayor que {ComparisonValue}.");            
            //LATER - make a custom validator for this property because a need to create custom placeholder
            RuleFor(x => x.MinimumLimitOfSowPerDay).GreaterThan(0).WithName("Siembra diaria mínima")
                .WithMessage("La configuración {PropertyName} debe ser mayor que {ComparisonValue}.")
                .LessThan(x => Convert.ToInt32(x.DailySowingPotential * 0.5))
                .WithMessage("La configuración {PropertyName} debe ser menor que {ComparisonValue}.");
            RuleFor(x => x.LocationMinimumSeedTray).InclusiveBetween(10, 100)
                .WithName("Bandejas mínima de una locación")
                .WithMessage("La configuración {PropertyName} debe estar entre {From} y {To}.");
            RuleFor(x => x.SeedlingMultiplier).ExclusiveBetween(1, 2)
                .WithName("Multiplicador de posturas")
                .WithMessage("La configuración {PropertyName} debe estar entre {From} y {To}.");
            RuleFor(x => x.SowShowRange).ExclusiveBetween(7,31)
                .WithName("Rango de muestra de siembras")
                .WithMessage("La configuración {PropertyName} debe estar entre {From} y {To}.");
            RuleFor(x => x.DeliveryShowRange).ExclusiveBetween(7, 31)
                .WithName("Rango de muestra de entregas")
                .WithMessage("La configuración {PropertyName} debe estar entre {From} y {To}.");
        }
    }
}
