using DataAccess.Repositories;
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
        ValidateSowChanges(orderLocation, date, sownSeedTrays);

        if (orderLocation.SeedTrayAmount == sownSeedTrays)
        {
            orderLocation.RealSowDate = date;
            _repository.Update(orderLocation);
        }
        else
        {
            OrderLocation orderLocationCopy = GetCopyOfAnOrderLocation(orderLocation);

            int alveolus = orderLocation.SeedlingAmount / orderLocation.SeedTrayAmount;

            orderLocation.SeedTrayAmount -= sownSeedTrays;
            orderLocation.SeedlingAmount = orderLocation.SeedTrayAmount * alveolus;

            orderLocationCopy.SeedTrayAmount = sownSeedTrays;
            orderLocationCopy.SeedlingAmount = orderLocationCopy.SeedTrayAmount * alveolus;
            orderLocationCopy.RealSowDate = date;

            _repository.Update(orderLocation);
            _repository.Insert(orderLocationCopy);
        }
    }

    private OrderLocation GetCopyOfAnOrderLocation(OrderLocation orderLocation)
    {
        return new OrderLocation()
        {
            Id = 0,
            GreenHouseId = orderLocation.GreenHouseId,
            SeedTrayId = orderLocation.SeedTrayId,
            OrderId = orderLocation.OrderId,
            SeedTrayAmount = orderLocation.SeedTrayAmount,
            SeedlingAmount = orderLocation.SeedlingAmount,
            EstimateSowDate = orderLocation.EstimateSowDate,
            EstimateDeliveryDate = orderLocation.EstimateDeliveryDate,
            RealSowDate = orderLocation.RealSowDate,
            RealDeliveryDate = orderLocation.RealDeliveryDate
        };
    }

    private void ValidateSowChanges(OrderLocation orderLocation, DateOnly date, int sownSeedTrays)
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

    //NEXT - make these two methods
    public void SavePlacedOrderLocationChange(OrderLocation orderLocationInProcess, int greenHouse, int block, short sownSeedTrays)
    {
        throw new NotImplementedException();
    }

    private void ValidatePlaceChanges(OrderLocation orderLocation, short placedSeedTrays)
    {
        if (placedSeedTrays <= 0 || placedSeedTrays > orderLocation.RestOfSeedTraysToBeLocated)
    {
            throw new ArgumentException("La cantidad de bandejas ubicadas debe estar entre 0 " +
                "y la cantidad de bandejas por ubicar de la Locación", "sownSeedTrays");
        }
        }
    }

    private enum OrderLocationType
    {
        WholeWithOutBrothers,
        PartialWithOutBrothers,
        WholeWithBrothers,
        PartialWithBrothers
    };
}


