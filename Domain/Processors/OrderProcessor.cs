using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using SupportLayer;
using SupportLayer.Models;
using System.Configuration;

namespace Domain.Processors;

public class OrderProcessor : IOrderProcessor
{
    //CHECK - If want to keep this interface type instead of change it to the class itself.
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

        if (_repository is OrderRepository orderRepository)
        {
            output = orderRepository.GetByARealSowDateOn(date)
                .OrderBy(x => x.EstimateSowDate)
                .ThenBy(x => x.DateOfRequest);
        }

        return output;
    }

    public IEnumerable<Order> GetNextOrdersToSow()
    {
        IEnumerable<Order> output = null!;

        int interval = int.Parse(ConfigurationManager.AppSettings[ConfigurationNames.SowShowRange]);

        DateOnly date = DateOnly.FromDateTime(DateTime.Now.AddDays(interval));

        output = _repository.GetIncompleteBeforeADate(date)
            .OrderBy(x => x.DateOfRequest)
            .ThenBy(x => x.EstimateSowDate);

        return output;
    }

    public void UpdateOrderStatusAfterSow(Order model)
    {
        bool edited = false;
        if (model.RealSowDate == null)
        {
            model.RealSowDate = model.OrderLocations.Min(x => x.RealSowDate);
            edited = true;
        }

        if (model.OrderLocations.All(x => x.RealSowDate != null))
        {
            model.Complete = true;
            edited = true;
        }

        if (edited == true)
            {
            _repository.Update(model);
        }
    }
}
