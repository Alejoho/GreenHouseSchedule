using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using SupportLayer.Models;

namespace Domain.Processors;

public class OrderProcessor
{
    private IOrderRepository _repository;
    public string Error { get; set; } = null!;

    public OrderProcessor()
    {
        _repository = new OrderRepository();
    }

    private bool ValidateData(Order model)
    {
        OrderValidator validator = new OrderValidator();
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

    public bool SaveOrder(Order model)
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

    public void DeleteOrder(int id)
    {
        _repository.Remove(id);
    }

    public IEnumerable<Order> GetAllOrders()
    {
        return _repository.GetAll();
    }

    public IEnumerable<Organization> GetFilteredOrganizations(string filter)
    {
        //_repository = new OrganizationRepository();
        //List<Organization> organizations = _repository.GetAll().ToList();

        //IEnumerable<Organization> output = organizations
        //    .Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
        //|| x.TypeOfOrganizationName.Contains(filter, StringComparison.OrdinalIgnoreCase)
        //|| x.MunicipalityName.Contains(filter, StringComparison.OrdinalIgnoreCase)
        //|| x.ProvinceName.Contains(filter, StringComparison.OrdinalIgnoreCase))
        //.OrderBy(x => x.Name);

        //return output;
        
        throw new NotImplementedException();
    }
}
