using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using SupportLayer.Models;

namespace Domain.Processors
{
    public class GreenHouseProcessor
    {
        private IGreenHouseRepository repository;
        public string Error { get; set; } = null!;

        public GreenHouseProcessor()
        {
            repository = new GreenHouseRepository();
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
                        repository.Insert(model);
                    }
                    else
                    {
                        repository.Update(model);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    //TODO - Add the code to log the errors
                    Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            return false;
        }

        public void DeleteGreenHouse(GreenHouse model)
        {
            //TODO - Create the delete method for green houses
            throw new NotImplementedException();
        }

        public IEnumerable<GreenHouse> GetAllGreenHouses()
        {
            //TODO - Create the GetAll method for green houses
            //throw new NotImplementedException();
            return repository.GetAll();
        }
    }
}
