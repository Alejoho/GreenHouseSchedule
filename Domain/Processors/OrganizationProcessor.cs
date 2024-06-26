using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using log4net;
using SupportLayer;
using SupportLayer.Models;

namespace Domain.Processors;

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
                ILog log = LogHelper.GetLogger();
                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(model);
                log.Error("There was an error saving an Organization record to the DB", ex);
                log4net.GlobalContext.Properties["Model"] = "";

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
        _repository = new OrganizationRepository();
        return _repository.GetAll().OrderBy(x => x.Name);
    }

    public IEnumerable<Organization> GetFilteredOrganizations(string filter)
    {
        _repository = new OrganizationRepository();
        List<Organization> organizations = _repository.GetAll().ToList();

        IEnumerable<Organization> output = organizations
            .Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
        || x.TypeOfOrganizationName.Contains(filter, StringComparison.OrdinalIgnoreCase)
        || x.MunicipalityName.Contains(filter, StringComparison.OrdinalIgnoreCase)
        || x.ProvinceName.Contains(filter, StringComparison.OrdinalIgnoreCase))
        .OrderBy(x => x.Name);

        return output;
    }
}
