using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using SupportLayer.Models;

namespace Domain.Processors
{
    public class SpeciesProcessor
    {
        private ISpeciesRepository _repository;
        public string Error { get; set; } = null!;
        public SpeciesProcessor()
        {
            _repository = new SpeciesRepository();
        }

        public bool ValidateData(Species model)
        {
            SpeciesValidator validator = new SpeciesValidator();
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

        public bool SaveSpecies(Species model)
        {
            if (ValidateData(model))
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
                    //NEXT - Add the code to log the errors
                    Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            return false;
        }

        public void DeleteSpecies(int id)
        {
            _repository.Remove(id);
        }

        public IEnumerable<Species> GetAllSpecies()
        {
            return _repository.GetAll().OrderBy(x => x.Name);
        }

        public Species GetASpeciesByID(int id)
        {
            Species output = ((SpeciesRepository)_repository).GetByID(id);
            return output;
        }
    }
}
