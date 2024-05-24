﻿using DataAccess.Repositories;
using SupportLayer.Models;

namespace Domain.Processors;

public class OrderLocationProcessor : IOrderLocationProcessor
{
    private OrderLocationRepository _repository;

    public OrderLocationProcessor()
    {
        _repository = new OrderLocationRepository();
    }

    public IEnumerable<OrderLocation> GetOrderLocationsFromADateOn(DateOnly date)
    {
        IEnumerable<OrderLocation> output = _repository.GetByASowDateOn(date)
            .OrderBy(x => x.RealSowDate)
            .ThenBy(x => x.Id);

        return output;
    }

    public void SaveSownOrderLocationChange(OrderLocation orderLocation, DateOnly date, short sownSeedTrays)
    {
        ValidateChanges(orderLocation, date, sownSeedTrays);
    }


    
    private void ValidateChanges(OrderLocation orderLocation, DateOnly date, int sownSeedTrays)
    {
        if (date > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("La fecha debe ser igual o anterior que el dia presente", "date");
        }

        if (sownSeedTrays <= 0 || sownSeedTrays > orderLocation.SeedTrayAmount)
        {
            throw new ArgumentException("La cantidad de bandejas sembradas debe estar entre 0 " +
                "y la cantidad de bandejas de la Locación", "sownSeedTrays");
    }
    }
}
