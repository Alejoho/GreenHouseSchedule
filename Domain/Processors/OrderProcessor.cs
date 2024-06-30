using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using log4net;
using SupportLayer;
using SupportLayer.Models;
using System.Configuration;

namespace Domain.Processors;

public class OrderProcessor : IOrderProcessor
{
    private ILog _log;
    private IOrderRepository _repository;
    public string Error { get; set; } = null!;

    public OrderProcessor()
    {
        _repository = new OrderRepository();
        _log = LogHelper.GetLogger();
    }

    public OrderProcessor(ILog log, IOrderRepository repository)
    {
        _repository = repository;
        _log = log;
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
                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(model);
                _log.Error("There was an error saving an Order record and all its OrderLocations records to the DB", ex);
                log4net.GlobalContext.Properties["Model"] = "";

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

    public void UpdateOrderStatusAfterSow(Order model, DateOnly date)
    {
        bool edited = false;
        if (model.RealSowDate == null)
        {
            model.RealSowDate = date;
            edited = true;
        }

        if (model.OrderLocations.All(x => x.RealSowDate != null))
        {
            model.Sown = true;
            edited = true;
        }

        if (edited == true)
        {
            _repository.Update(model);

            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(model);
            _log.Info("An Order record was updated to the DB after sow");
            log4net.GlobalContext.Properties["Model"] = "";
        }
    }

    public IEnumerable<Order> GetNextOrdersToUnload()
    {
        var output = _repository.GetSownsWithoutPlace().OrderByDescending(x => x.RealSowDate);
        return output;
    }

    public IEnumerable<Order> GetNextOrdersToDeliver()
    {
        IEnumerable<Order> output = null!;

        int interval = int.Parse(ConfigurationManager.AppSettings[ConfigurationNames.DeliveryShowRange]);

        DateOnly date = DateOnly.FromDateTime(DateTime.Now.AddDays(interval));

        output = _repository.GetReadyToDeliver(date)
            .OrderBy(x => x.EstimateDeliveryDate)
            .ThenBy(x => x.DateOfRequest);

        return output;
    }

    public void UpdateOrderStatusAfterDelivery(Order model)
    {
        bool markAsDelivered = true;

        foreach (OrderLocation orderLocation in model.OrderLocations)
        {
            foreach (Block block in orderLocation.Blocks)
            {
                int seedTraysAlreadyDelivered = block.DeliveryDetails.Sum(x => x.SeedTrayAmountDelivered);
                int seedTraysToBeDelivered = block.SeedTrayAmount - seedTraysAlreadyDelivered;

                if (seedTraysToBeDelivered > 0)
                {
                    markAsDelivered = false;
                    return;
                }
            }
        }

        if (markAsDelivered == true)
        {
            model.Delivered = true;
            _repository.Update(model);
        }
    }

    public IEnumerable<Order> GetOrdersInTheSeedBed()
    {
        var output = _repository.GetSownsWithPlace().OrderBy(x => x.RealSowDate);

        return output;
    }
}
