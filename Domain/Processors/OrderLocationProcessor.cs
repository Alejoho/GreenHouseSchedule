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

    private void SaveCompleteWithoutBrothersOrderLocation(OrderLocation orderLocation, byte greenHouse, byte block, short placedSeedTrays)
    {
        orderLocation.GreenHouseId = greenHouse;
        _repository.Update(orderLocation);

        Block blockEntity = new Block()
        {
            OrderLocationId = orderLocation.Id,
            BlockNumber = block,
            SeedTrayAmount = placedSeedTrays
        };

        BlockRepository blockRepository = new BlockRepository();
        blockRepository.Insert(blockEntity);
    }

    private void SavePartialWithoutBrothersOrderLocation(OrderLocation orderLocation, byte greenHouse, byte block, short placedSeedTrays)
    {
        //1-crear una copia del orderlocation
        OrderLocation orderLocationCopy = GetCopyOfAnOrderLocation(orderLocation);

        //2-asignarle a la copia sus valores(casa, cantidad de bandejas, cantidad de posturas)
        orderLocationCopy.GreenHouseId = greenHouse;
        int alveolus = orderLocation.SeedlingAmount / orderLocation.SeedTrayAmount;
        orderLocationCopy.SeedTrayAmount = placedSeedTrays;
        orderLocationCopy.SeedlingAmount = placedSeedTrays * alveolus;

        //3-salvar la copia y recuperar su id
        //NEXT - Check if this id is retrieve from the DB
        _repository.Insert(orderLocationCopy);

        //4-reducir la cantidad de bandejas y posturas del OL original y salvarlo a la DB
        orderLocation.SeedTrayAmount -= orderLocationCopy.SeedTrayAmount;
        orderLocation.SeedlingAmount -= orderLocationCopy.SeedlingAmount;
        _repository.Update(orderLocation);

        //5-crear un block object, asignarle sus valores(OLid, blocknumber and seedtrayamount) en base al OLCopy
        //y salvarlo en la DB.

        Block blockEntity = new Block()
        {
            OrderLocationId = orderLocationCopy.Id,
            BlockNumber = block,
            SeedTrayAmount = placedSeedTrays
        };

        BlockRepository blockRepository = new BlockRepository();
        blockRepository.Insert(blockEntity);
    }

    private void SaveCompleteWithBrothersOrderLocation(OrderLocation orderLocation, byte greenHouse, byte block, short placedSeedTrays)
    {
        //encontrar el hermano
        OrderLocation orderLocationBrother = GetOrderLocationsBrother(orderLocation, greenHouse);

        //agregar los datos al hermano
        orderLocationBrother.SeedTrayAmount += orderLocation.SeedTrayAmount;
        orderLocationBrother.SeedlingAmount += orderLocation.SeedlingAmount;

        //salvar el hermano
        _repository.Update(orderLocationBrother);

        //eliminar el original
        _repository.Remove(orderLocation.Id);
    }

    private OrderLocation GetOrderLocationsBrother(OrderLocation orderLocation, byte greenHouse)
    {
        Order order = orderLocation.Order;

        var output = order.OrderLocations.Where(x =>
                x.GreenHouseId == greenHouse
                && x.SeedTrayId == orderLocation.SeedTrayId
                && x.RealSowDate == orderLocation.RealSowDate);

        if (output.Count() != 1)
        {
            throw new ApplicationException("There's more than 1 order location brother.");
        }

        return output.First();
    }

    public void SavePlacedOrderLocationChange(OrderLocation orderLocationInProcess, byte greenHouse, byte block, short placedSeedTrays)
    {
        ValidatePlaceChanges(orderLocationInProcess, placedSeedTrays);

        switch (DetermineOrderLocationType(orderLocationInProcess, greenHouse, placedSeedTrays))
        {
            case OrderLocationType.CompleteWithoutBrothers:

                SaveCompleteWithoutBrothersOrderLocation(orderLocationInProcess, greenHouse, block, placedSeedTrays);

                break;

            case OrderLocationType.PartialWithoutBrothers:

                SavePartialWithoutBrothersOrderLocation(orderLocationInProcess, greenHouse, block, placedSeedTrays);

                break;

            case OrderLocationType.CompleteWithBrothers:

                SaveCompleteWithBrothersOrderLocation(orderLocationInProcess, greenHouse, block, placedSeedTrays);

                break;

            case OrderLocationType.PartialWithBrothers:

                break;

        }

    }

    private OrderLocationType DetermineOrderLocationType(OrderLocation orderLocation, byte greenHouse, short seedTrays)
    {
        Order order = orderLocation.Order;

        bool isThereBrother = order.OrderLocations
            .Any(x =>
                x.GreenHouseId == greenHouse
                && x.SeedTrayId == orderLocation.SeedTrayId
                && x.RealSowDate == orderLocation.RealSowDate
            );

        if (isThereBrother == false && seedTrays == orderLocation.SeedTrayAmount)
            {
            return OrderLocationType.CompleteWithoutBrothers;
            }
        else if (isThereBrother == false && seedTrays < orderLocation.SeedTrayAmount)
            {
            return OrderLocationType.PartialWithoutBrothers;
            }
        else if (isThereBrother == true && seedTrays == orderLocation.SeedTrayAmount)
            {
            return OrderLocationType.CompleteWithBrothers;
            }
        else if (isThereBrother == true && seedTrays < orderLocation.SeedTrayAmount)
            {
                return OrderLocationType.PartialWithBrothers;
            }

        throw new Exception("Tipo the locacion no encontrada");
    }
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
        CompleteWithoutBrothers,
        PartialWithoutBrothers,
        CompleteWithBrothers,
        PartialWithBrothers
    };
}


