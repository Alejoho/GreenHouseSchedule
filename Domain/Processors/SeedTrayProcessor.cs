using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using log4net;
using SupportLayer;
using SupportLayer.Models;

namespace Domain.Processors
{
    public class SeedTrayProcessor
    {
        private ISeedTrayRepository _repository;
        public string Error { get; set; } = null!;

        public SeedTrayProcessor()
        {
            _repository = new SeedTrayRepository();
        }

        private bool ValidateData(SeedTray model)
        {
            SeedTrayValidator validator = new SeedTrayValidator();
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

        public bool SaveSeedTray(SeedTray model)
        {
            if (ValidateData(model) == true)
            {
                try
                {
                    if (model.Id == 0)
                    {
                        _repository.Insert(model);
                    }
                    else
                    {
                        _repository.Update(model);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    ILog log = LogHelper.GetLogger();
                    log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(model);
                    log.Error("There was an error saving a SeedTray record to the DB", ex);
                    log4net.GlobalContext.Properties["Model"] = "";

                    Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            return false;
        }

        public void DeleteSeedTray(int id)
        {
            _repository.Remove(id);
        }

        public IEnumerable<SeedTray> GetAllSeedTrays()
        {
            return _repository.GetAll();
        }

        public IEnumerable<SeedTray> GetActiveSeedTrays()
        {
            return _repository.GetAll().Where(x => x.Active == true)
                .OrderBy(x => x.Name);
        }

        public void CheckChangeInTheSelection(List<SeedTray> seedTrays)
        {
            var changedSeedTrays = seedTrays.Where(x => x.IsSelected != x.Selected);

            foreach (var seedTray in changedSeedTrays)
            {
                seedTray.Selected = seedTray.IsSelected;
                this.SaveSeedTray(seedTray);

                ILog log = LogHelper.GetLogger();
                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(seedTray);
                log.Info("A SeedTray seletion was updated to the DB");
                log4net.GlobalContext.Properties["Model"] = "";
            }
        }
    }
}
