using FluentValidation;
using SupportLayer.Models;
using System.Text.RegularExpressions;

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
        RuleFor(x => x.PhoneNumber)
            .Must(x => Regex.IsMatch(x, @"\+53[0-9]{8}$") || Regex.IsMatch(x, @"^\d{8}$"))
            .When(x => x.PhoneNumber != "")
            .WithName("Número celular")
            .WithMessage("El {PropertyName} no está en el formato correcto." +
                "\nFormato correcto: +53xxxxxxxx ó xxxxxxxx");
        RuleFor(x => x.OtherNumber)
            .Must(x => Regex.IsMatch(x, @"\+53[0-9]{8}$") || Regex.IsMatch(x, @"^\d{8}$"))
            .When(x => x.OtherNumber != "")
            .WithName("Número fijo")
            .WithMessage("El {PropertyName} no está en el formato correcto." +
                "\nFormato correcto: +53xxxxxxxx ó xxxxxxxx");
        RuleFor(x => x.OrganizationId).GreaterThan((short)0).WithName("Organización")
            .WithMessage("La {PropertyName} no debe estar vacía.");
    }
}
