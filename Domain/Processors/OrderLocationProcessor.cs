using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.ValuableObjects;
using log4net;
using SupportLayer;
using SupportLayer.Models;

namespace Domain.Processors;

public class OrderLocationProcessor : IOrderLocationProcessor
{
    private static ILog _log;
    private IOrderLocationRepository _orderLocationRepository;
    private IBlockRepository _blockRepository;

    public OrderLocationProcessor()
    {
        _orderLocationRepository = new OrderLocationRepository();
        _blockRepository = new BlockRepository();
        _log = LogHelper.GetLogger();
    }

    public OrderLocationProcessor(ILog log, IOrderLocationRepository _orderLocationRepository, IBlockRepository blockRepository)
    {
        _log = log;
        this._orderLocationRepository = _orderLocationRepository;
        _blockRepository = blockRepository;
    }

    public IEnumerable<OrderLocation> GetOrderLocationsFromADateOn(DateOnly date)
    {
        IEnumerable<OrderLocation> output = _orderLocationRepository.GetByASowDateOn(date)
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
            _orderLocationRepository.Update(orderLocation);

            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(orderLocation);
            _log.Info("An OrderLocation was sown and updated to the DB");
            log4net.GlobalContext.Properties["Model"] = "";
        }
        else
        {
            OrderLocation orderLocationCopy = HelpingMethods.GetCopyOfAnOrderLocation(orderLocation);

            int alveolus = orderLocation.SeedlingAmount / orderLocation.SeedTrayAmount;

            orderLocation.SeedTrayAmount -= sownSeedTrays;
            orderLocation.SeedlingAmount = orderLocation.SeedTrayAmount * alveolus;

            orderLocationCopy.SeedTrayAmount = sownSeedTrays;
            orderLocationCopy.SeedlingAmount = orderLocationCopy.SeedTrayAmount * alveolus;
            orderLocationCopy.RealSowDate = date;

            _orderLocationRepository.Update(orderLocation);
            _orderLocationRepository.Insert(orderLocationCopy);

            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(orderLocationCopy);
            _log.Info("Part of an OrderLocation was sown and inserted to the DB");
            log4net.GlobalContext.Properties["Model"] = "";
        }
    }

    private void ValidateSowChanges(OrderLocation orderLocation, DateOnly date, int sownSeedTrays)
    {
        if (date > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("La fecha debe ser igual o anterior que el día presente", "date");
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
        _orderLocationRepository.Update(orderLocation);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(orderLocation);
        _log.Info("Updated the GreenHouse of the OrderLocation to the DB. SaveCompleteWithoutBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";

        Block blockEntity = new Block()
        {
            OrderLocationId = orderLocation.Id,
            BlockNumber = block,
            SeedTrayAmount = placedSeedTrays
        };

        _blockRepository.Insert(blockEntity);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(blockEntity);
        _log.Info("Added a Block to the OrderLocation in the DB. SaveCompleteWithoutBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";
    }

    private void SavePartialWithoutBrothersOrderLocation(OrderLocation orderLocation, byte greenHouse, byte block, short placedSeedTrays)
    {
        //1-crear una copia del orderlocation
        OrderLocation orderLocationCopy = HelpingMethods.GetCopyOfAnOrderLocation(orderLocation);

        //2-asignarle a la copia sus valores(casa, cantidad de bandejas, cantidad de posturas)
        orderLocationCopy.GreenHouseId = greenHouse;
        int alveolus = orderLocation.SeedlingAmount / orderLocation.SeedTrayAmount;
        orderLocationCopy.SeedTrayAmount = placedSeedTrays;
        orderLocationCopy.SeedlingAmount = placedSeedTrays * alveolus;

        //3-salvar la copia y recuperar su id
        _orderLocationRepository.Insert(orderLocationCopy);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(orderLocationCopy);
        _log.Info("Inserted an OrderLocation copy to the DB. SavePartialWithoutBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";

        //4-reducir la cantidad de bandejas y posturas del OL original y salvarlo a la DB
        orderLocation.SeedTrayAmount -= orderLocationCopy.SeedTrayAmount;
        orderLocation.SeedlingAmount -= orderLocationCopy.SeedlingAmount;
        _orderLocationRepository.Update(orderLocation);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(orderLocation);
        _log.Info("Updated the original OrderLocation to the DB. SavePartialWithoutBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";

        //5-crear un block object, asignarle sus valores(OLid, blocknumber and seedtrayamount) en base al OLCopy
        //y salvarlo en la DB.

        Block blockEntity = new Block()
        {
            OrderLocationId = orderLocationCopy.Id,
            BlockNumber = block,
            SeedTrayAmount = placedSeedTrays
        };

        _blockRepository.Insert(blockEntity);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(blockEntity);
        _log.Info("Added a Block to the OrderLocation in the DB. SavePartialWithoutBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";
    }

    private void SaveCompleteWithBrothersOrderLocation(OrderLocation orderLocation, byte greenHouse, byte block, short placedSeedTrays)
    {
        //encontrar el hermano
        OrderLocation orderLocationBrother = HelpingMethods.GetOrderLocationsBrother(orderLocation, greenHouse);

        //agregar los datos al hermano
        orderLocationBrother.SeedTrayAmount += orderLocation.SeedTrayAmount;
        orderLocationBrother.SeedlingAmount += orderLocation.SeedlingAmount;

        //salvar el hermano
        _orderLocationRepository.Update(orderLocationBrother);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(orderLocationBrother);
        _log.Info("Updated an OrderLocation brother to the DB. SaveCompleteWithBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";

        //crear el nuevo block y salvarlo
        Block blockEntity = new Block()
        {
            OrderLocationId = orderLocationBrother.Id,
            BlockNumber = block,
            SeedTrayAmount = placedSeedTrays
        };

        _blockRepository.Insert(blockEntity);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(blockEntity);
        _log.Info("Added a Block to the OrderLocation brother in the DB. SaveCompleteWithBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";

        //eliminar el original
        _orderLocationRepository.Remove(orderLocation.Id);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(orderLocation);
        _log.Info("Removed the OrderLocation original from the DB. SaveCompleteWithBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";
    }

    private void SavePartialWithBrothersOrderLocation(OrderLocation orderLocation, byte greenHouse, byte block, short placedSeedTrays)
    {
        //encontrar el hermano y los alveolos de la bandeja usadadas
        OrderLocation orderLocationBrother = HelpingMethods.GetOrderLocationsBrother(orderLocation, greenHouse);

        int alveolus = orderLocation.SeedlingAmount / orderLocation.SeedTrayAmount;

        //asignar editar los valores en los dos objetos(sumar al hermano y quitar al original)
        orderLocationBrother.SeedTrayAmount += placedSeedTrays;
        orderLocationBrother.SeedlingAmount += placedSeedTrays * alveolus;

        orderLocation.SeedTrayAmount -= placedSeedTrays;
        orderLocation.SeedlingAmount -= placedSeedTrays * alveolus;

        //actualizar ambos objetos en la DB
        _orderLocationRepository.Update(orderLocation);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(orderLocation);
        _log.Info("Updated the original OrderLocation to the DB. SavePartialWithBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";

        _orderLocationRepository.Update(orderLocationBrother);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(orderLocationBrother);
        _log.Info("Updated an OrderLocation brother to the DB. SavePartialWithBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";

        //crear el nuevo bloque y salvarlo
        Block blockEntity = new Block()
        {
            OrderLocationId = orderLocationBrother.Id,
            BlockNumber = block,
            SeedTrayAmount = placedSeedTrays
        };

        _blockRepository.Insert(blockEntity);

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(blockEntity);
        _log.Info("Added a Block to the OrderLocation brother in the DB. SavePartialWithBrothersOrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";
    }

    public void SavePlacedOrderLocationChange(OrderLocation orderLocationInProcess, byte greenHouse, byte block, short placedSeedTrays)
    {
        //brother OL are OL's that have the same real sow date, the same seedtray type and the same greenhouse

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

                SavePartialWithBrothersOrderLocation(orderLocationInProcess, greenHouse, block, placedSeedTrays);

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

    private void ValidatePlaceChanges(OrderLocation orderLocation, short placedSeedTrays)
    {
        int restOfSeedTraysToBeLocated = orderLocation.SeedTrayAmount - orderLocation.Blocks.Sum(x => x.SeedTrayAmount);

        if (placedSeedTrays <= 0 || placedSeedTrays > restOfSeedTraysToBeLocated)
        {
            throw new ArgumentException("La cantidad de bandejas ubicadas debe estar entre 0 " +
                "y la cantidad de bandejas por ubicar de la locación", "placedSeedTrays");
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


