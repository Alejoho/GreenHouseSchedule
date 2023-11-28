using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation;
using FluentValidation.Results;
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
                //LATER - Add the code to log the errors
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
        return _repository.GetAll().OrderBy(x => x.Name);
    }

    public IEnumerable<Client> GetFilteredClients()
    {
        throw new NotImplementedException();
        //List<Client> organizations = _repository.GetAll().ToList();
        //IEnumerable<Client> output;
        //return output;
    }
}
