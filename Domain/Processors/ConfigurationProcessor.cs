using Domain.Validators;
using FluentValidation.Results;
using System.Configuration;

namespace Domain.Processors
{
    public class ConfigurationProcessor
    {
        public string Error { get; set; } = null!;
        private readonly string UserSettingsDirectory;

        public ConfigurationProcessor()
        {
            UserSettingsDirectory = GetUserSettingsDirectory();
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
                string configFilePath = GetUserSettingsDirectory();
                ExeConfigurationFileMap configFileMap =
                    new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = configFilePath;

                Configuration config = ConfigurationManager
                    .OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                //Configuration config = ConfigurationManager
                //.OpenExeConfiguration(UserSettingsDirectory);
                //EXT - Object not set to an instance of an object
                string algo = config.AppSettings.Settings["RegressionDaysq"].Value;
                //config.AppSettings.Settings["RegressionDaysq"].Value = 
                //    model.RegressionDays.ToString();
                //ConfigurationManager.AppSettings[ConfigurationNames.RegressionDays] =
                //    model.RegressionDays.ToString();
                //ConfigurationManager.AppSettings[ConfigurationNames.DailySowingPotential] =
                //    model.DailySowingPotential.ToString();
                //ConfigurationManager.AppSettings[ConfigurationNames.MinimumLimitOfSowPerDay] =
                //    model.MinimumLimitOfSowPerDay.ToString();
                //ConfigurationManager.AppSettings[ConfigurationNames.LocationMinimumSeedTray] =
                //    model.LocationMinimumSeedTray.ToString();
                //ConfigurationManager.AppSettings[ConfigurationNames.SeedlingMultiplier] =
                //    model.SeedlingMultiplier.ToString();
                //ConfigurationManager.AppSettings[ConfigurationNames.SowShowRange] =
                //    model.SowShowRange.ToString();
                //ConfigurationManager.AppSettings[ConfigurationNames.DeliveryShowRange] =
                //    model.DeliveryShowRange.ToString();

                config.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("appSettings");

                return true;
            }
            return false;
        }

        private string GetUserSettingsDirectory()
        {
            string output;
            string dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string nameOfTheFile = "UserSettings.config";
            output = dataDirectory + nameOfTheFile;

            return output;
        }

        public Configurations GetConfigurations()
        {
            throw new NotImplementedException();
        }
    }
}
