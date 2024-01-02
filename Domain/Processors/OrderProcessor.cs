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

    public IEnumerable<Order> GetFilteredOrders(string filter)
    {
        _repository = new OrderRepository();
        List<Order> orders = _repository.GetAll().ToList();

        IEnumerable<Order> output = orders
            .Where(x => x.Client.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.Product.Specie.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.Product.Variety.Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.AmountOfWishedSeedlings.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.AmountOfAlgorithmSeedlings.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.WishDate.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.DateOfRequest.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.EstimateSowDate.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.EstimateDeliveryDate.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.RealSowDate.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)
            || x.RealDeliveryDate.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)
            ).OrderBy(x => x.Client.Name);

        return output;       
    }

    public IEnumerable<Order> GetOrdersFromADateOn(DateOnly date)
    {
        IEnumerable<Order> output = null!;

        if(_repository is OrderRepository orderRepository)
        {
            output = orderRepository.GetByDateOn(date)
                .OrderBy(x => x.EstimateSowDate)
                .ThenBy(x => x.DateOfRequest);
        }

        return output;
    }
}
