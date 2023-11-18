using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using SupportLayer.Models;

namespace Domain.Processors
{
    public class MunicipalityProcessor
    {
        //CHECK - do this processor
        private IGenericRepository<Municipality> _repository;
        public string Error { get; set; } = null!;

        public MunicipalityProcessor()
        {
            _repository = new MunicipalityRepository();
        }

        private bool ValidateData(Municipality model)
        {
            MunicipalityValidator validator = new MunicipalityValidator();
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

        public bool SaveMunicipality(Municipality model)
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
                    //LATER - Add the code to log the errors
                    Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            return false;
        }

        public void DeleteMunicipality(int id)
        {
            _repository.Remove(id);
        }

        public IEnumerable<Municipality> GetAllMunicipalities()
        {
            //TODO - do the ordering in all the processors 
            return _repository.GetAll().OrderBy(x => x.ProvinceName).ThenBy(x => x.Name);
        }
    }
}
