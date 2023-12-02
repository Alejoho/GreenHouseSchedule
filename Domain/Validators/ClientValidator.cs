using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators;

public class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator()
	{
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name).NotEmpty().WithName("Nombre")
            .WithMessage("El {PropertyName} no debe estar vacío.")
            .MaximumLength(50)
            .WithMessage("El {PropertyName} no debe exceder los 50 caracteres.");
        RuleFor(x => x.NickName).MaximumLength(50).When(x => x.NickName != "")
            .WithName("Apodo")
            .WithMessage("El {PropertyName} no debe exceder los 50 caracteres.");
        //NEXT - use a regular expresion to validate the numbers
        RuleFor(x => x.PhoneNumber).MaximumLength(20).When(x => x.PhoneNumber != "")
            .WithName("Celular")
            .WithMessage("El {PropertyName} no debe exceder los 20 caracteres.");
        RuleFor(x => x.OtherNumber).MaximumLength(20).When(x => x.OtherNumber != "")
            .WithName("Teléfono")
            .WithMessage("El {PropertyName} no debe exceder los 20 caracteres.");
        RuleFor(x => x.OrganizationId).GreaterThan((short)0).WithName("Organización")
            .WithMessage("La {PropertyName} no debe estar vacía.");
    }
}
