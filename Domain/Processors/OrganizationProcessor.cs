using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using SupportLayer.Models;

namespace Domain.Processors
{
    public class OrganizationProcessor
    {
        private IOrganizationRepository _repository;
        public string Error { get; set; } = null!;

        public OrganizationProcessor()
        {
            _repository = new OrganizationRepository();
        }

        private bool ValidateData(Organization model)
        {
            OrganizationValidator validator = new OrganizationValidator();
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

        public bool SaveOrganization(Organization model)
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

        public void DeleteOrganization(int id)
        {
            _repository.Remove(id);
        }

        public IEnumerable<Organization> GetAllOrganizations()
        {
            return _repository.GetAll().OrderBy(x => x.Name);
        }
    }
}
