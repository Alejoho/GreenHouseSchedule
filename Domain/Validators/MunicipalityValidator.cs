using FluentValidation;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validators
{
    public class MunicipalityValidator : AbstractValidator<Municipality>
    {
        public MunicipalityValidator()
        {
            //NEXT - do the ctor of the validator of municipalities
        }
    }
}
