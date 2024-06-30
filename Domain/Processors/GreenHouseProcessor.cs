using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using log4net;
using SupportLayer;
using SupportLayer.Models;

namespace Domain.Processors
{
    public class GreenHouseProcessor
    {
        private IGreenHouseRepository _repository;
        public string Error { get; set; } = null!;

        public GreenHouseProcessor()
        {
            _repository = new GreenHouseRepository();
        }

        private bool ValidateData(GreenHouse model)
        {
            GreenHouseValidator validator = new GreenHouseValidator();
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

        public bool SaveGreenHouse(GreenHouse model)
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
                    log.Error("There was an error saving a GreenHouse record to the DB", ex);
                    log4net.GlobalContext.Properties["Model"] = "";

                    Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            return false;
        }

        public void DeleteGreenHouse(int id)
        {
            _repository.Remove(id);
        }

        public IEnumerable<GreenHouse> GetAllGreenHouses()
        {
            return _repository.GetAll().OrderBy(x => x.Name);
        }

        public IEnumerable<GreenHouse> GetActiveGreenHouses()
        {
            return _repository.GetOnlyActive().OrderBy(x => x.Name);
        }
    }
}
