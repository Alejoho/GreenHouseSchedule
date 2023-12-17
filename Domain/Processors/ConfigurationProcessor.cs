using Domain.Validators;
using FluentValidation.Results;
using SupportLayer;
using SupportLayer.Models;
using System.Configuration;

namespace Domain.Processors
{
    public class ConfigurationProcessor
    {
        public string Error { get; set; } = null!;

        public ConfigurationProcessor()
        {
        }

        private bool ValidateData(Configurations model)
        {
            ConfigurationValidator validator = new ConfigurationValidator();
            ValidationResult validationResult = validator.Validate(model);

            if (validationResult.IsValid)
            {
                return true;
            }
            else
            {
                Error = validationResult.Errors.First().ErrorMessage;
                return false;
            }
        }

        public bool SaveConfigurations(Configurations model)
        {
            if (ValidateData(model) == true)
            {
                ConfigurationManager.AppSettings[ConfigurationNames.RegressionDays] =
                    model.RegressionDays.ToString();
                ConfigurationManager.AppSettings[ConfigurationNames.DailySowingPotential] =
                    model.DailySowingPotential.ToString();
                ConfigurationManager.AppSettings[ConfigurationNames.MinimumLimitOfSowPerDay] =
                    model.MinimumLimitOfSowPerDay.ToString();
                ConfigurationManager.AppSettings[ConfigurationNames.LocationMinimumSeedTray] =
                    model.LocationMinimumSeedTray.ToString();
                ConfigurationManager.AppSettings[ConfigurationNames.SeedlingMultiplier] =
                    model.SeedlingMultiplier.ToString();
                ConfigurationManager.AppSettings[ConfigurationNames.SowShowRange] =
                    model.SowShowRange.ToString();
                ConfigurationManager.AppSettings[ConfigurationNames.DeliveryShowRange] =
                    model.DeliveryShowRange.ToString();
                return true;
            }
            return false;
        }

        public Configurations GetConfigurations()
        {
            throw new NotImplementedException();
        }
    }
}
