using FluentValidation;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validators
{
    internal class SeedTrayValidator : AbstractValidator<SeedTray>
    {
        public SeedTrayValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;


        }
    }
}
