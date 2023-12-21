using FluentValidation;
using SupportLayer.Models;

namespace Domain.Validators;

public class OrderValidator : AbstractValidator<Order>
{
    public OrderValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
    }
}
