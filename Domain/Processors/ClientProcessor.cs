using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation;
using FluentValidation.Results;
using log4net;
using SupportLayer;
using SupportLayer.Models;

namespace Domain.Processors;

public class ClientProcessor
{
    private IClientRepository _repository;
    public string Error { get; set; } = null!;

    public ClientProcessor()
    {
        _repository = new ClientRepository();
    }

    private bool ValidateData(Client model)
    {
        ClientValidator validator = new ClientValidator();
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

    public bool SaveClient(Client model)
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
                log.Error("There was an error saving a Client record to the DB", ex);
                log4net.GlobalContext.Properties["Model"] = "";

                Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return false;
            }
        }

        return false;
    }

    public void DeleteClient(int id)
    {
        _repository.Remove(id);
    }

    public IEnumerable<Client> GetAllClients()
    {
        _repository = new ClientRepository();
        return _repository.GetAll().OrderBy(x => x.Name);
    }

    public IEnumerable<Client> GetFilteredClients(string filter)
    {
        _repository = new ClientRepository();
        List<Client> clients = _repository.GetAll().ToList();

        IEnumerable<Client> output = clients
            .Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.NickName.Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.PhoneNumber.Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.OtherNumber.Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.TypeAndOrganizationName.Contains(filter, StringComparison.OrdinalIgnoreCase)
            ).OrderBy(x => x.Name);

        return output;
    }
}
