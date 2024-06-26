using DataAccess.Repositories;
using Domain.ValuableObjects;
using log4net;
using SupportLayer;
using SupportLayer.Models;

namespace Domain.Processors;


public class BlockProcessor
{
    private static readonly ILog _log = LogHelper.GetLogger();
    private BlockRepository _repository;
    public BlockProcessor()
    {
        _repository = new BlockRepository();
    }

    private void UpdateBlockPlaceInAHouse(Block blockInProcess, byte block, short relocatedSeedTrays)
    {
        OrderLocation orderlocation = blockInProcess.OrderLocation;

        Block newBlock = new Block()
        {
            OrderLocationId = orderlocation.Id,
            BlockNumber = block,
            SeedTrayAmount = relocatedSeedTrays
        };

        _repository.Insert(newBlock);

        blockInProcess.SeedTrayAmount -= relocatedSeedTrays;

        if (blockInProcess.SeedTrayAmount == 0)
        {
            _repository.Remove(blockInProcess.Id);
            blockInProcess.OrderLocation.Blocks.Remove(blockInProcess);
        }
        else
        {
            _repository.Update(blockInProcess);
        }

        orderlocation.Blocks.Add(newBlock);
        newBlock.OrderLocation = orderlocation;

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(newBlock);
        _log.Info("A Block was relocated completely or partial in the same " +
            "GreenHouse and updated to the DB. UpdateBlockPlaceInAHouse");
        log4net.GlobalContext.Properties["Model"] = "";
    }

    private void TransferBlock(OrderLocation reciever, Block blockInProcess, byte block, short relocatedSeedTrays)
    {
        //obtengo el sender
        OrderLocation sender = blockInProcess.OrderLocation;

        //actualizar las bandejas y las posturas en el reciever
        int alveolus = sender.SeedlingAmount / sender.SeedTrayAmount;
        reciever.SeedTrayAmount += relocatedSeedTrays;
        reciever.SeedlingAmount += relocatedSeedTrays * alveolus;

        //actualizo el reciever en la DB
        OrderLocationRepository orderLocationRepository = new OrderLocationRepository();

        orderLocationRepository.Update(reciever);

        //creo el nuevo bloque y lo inserto en la DB
        Block newBlock = new Block()
        {
            OrderLocationId = reciever.Id,
            BlockNumber = block,
            SeedTrayAmount = relocatedSeedTrays
        };

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(newBlock);
        _log.Info("A Block was transfer to another " +
            "GreenHouse and updated to the DB");
        log4net.GlobalContext.Properties["Model"] = "";

        _repository.Insert(newBlock);

        //actualizo el bloque original guardo cambios en la DB
        blockInProcess.SeedTrayAmount -= relocatedSeedTrays;

        if (blockInProcess.SeedTrayAmount == 0)
        {
            _repository.Remove(blockInProcess.Id);
            sender.Blocks.Remove(blockInProcess);
        }
        else
        {
            _repository.Update(blockInProcess);
        }

        //actualizo el sender en la DB
        sender.SeedTrayAmount -= relocatedSeedTrays;
        sender.SeedlingAmount -= relocatedSeedTrays * alveolus;

        if (sender.SeedTrayAmount == 0)
        {
            orderLocationRepository.Remove(sender.Id);
            sender.Order.OrderLocations.Remove(sender);
        }
        else
        {
            orderLocationRepository.Update(sender);
        }

        reciever.Blocks.Add(newBlock);
        newBlock.OrderLocation = reciever;
    }

    private void UpdateBlockPlaceOutAHouseWithBrother(Block blockInProcess, byte greenHouse, byte block, short relocatedSeedTrays)
    {
        //LATER - remover estos comentarios

        //consiguo el orderlocation brother y el original
        OrderLocation orderLocationBrother = HelpingMethods.GetOrderLocationsBrother(blockInProcess.OrderLocation, greenHouse);

        //llamo a este metodo
        TransferBlock(orderLocationBrother, blockInProcess, block, relocatedSeedTrays);

        _log.Info("Completed the relocation of the block. UpdateBlockPlaceOutAHouseWithBrother");
    }

    private void UpdateBlockPlaceOutAHouseWithOutBrother(Block blockInProcess, byte greenHouse, byte block, short relocatedSeedTrays)
    {
        OrderLocation orderLocationCopy = HelpingMethods.GetCopyOfAnOrderLocation(blockInProcess.OrderLocation);

        orderLocationCopy.GreenHouseId = greenHouse;
        orderLocationCopy.SeedTrayAmount = 0;
        orderLocationCopy.SeedlingAmount = 0;

        OrderLocationRepository orderLocationRepository = new OrderLocationRepository();

        orderLocationRepository.Insert(orderLocationCopy);
        orderLocationCopy.SeedTray = blockInProcess.OrderLocation.SeedTray;
        orderLocationCopy.Order = blockInProcess.OrderLocation.Order;

        GreenHouseRepository greenHouseRepository = new GreenHouseRepository();
        orderLocationCopy.GreenHouse = greenHouseRepository.GetAll().First(x => x.Id == greenHouse);

        blockInProcess.OrderLocation.Order.OrderLocations.Add(orderLocationCopy);

        TransferBlock(orderLocationCopy, blockInProcess, block, relocatedSeedTrays);

        _log.Info("Completed the relocation of the block. UpdateBlockPlaceOutAHouseWithOutBrother");
    }

    public void SaveRelocateBlockChange(Block blockInProcess, byte greenHouse, byte block, short relocatedSeedTrays)
    {
        ValidateRelocateChanges(blockInProcess, relocatedSeedTrays);

        MovementType movementType = DetermineMovementType(blockInProcess, greenHouse, relocatedSeedTrays);

        switch (movementType)
        {
            case MovementType.CompleteInAHouse:
                UpdateBlockPlaceInAHouse(blockInProcess, block, relocatedSeedTrays);
                break;
            case MovementType.PartialInAHouse:
                UpdateBlockPlaceInAHouse(blockInProcess, block, relocatedSeedTrays);
                break;

            case MovementType.WithBortherOutAHouse:
                UpdateBlockPlaceOutAHouseWithBrother(blockInProcess, greenHouse, block, relocatedSeedTrays);
                break;

            case MovementType.WithoutBortherOutAHouse:
                UpdateBlockPlaceOutAHouseWithOutBrother(blockInProcess, greenHouse, block, relocatedSeedTrays);
                break;

        }
    }

    private MovementType DetermineMovementType(Block blockInProcess, byte greenHouse, short relocatedSeedTrays)
    {
        if (blockInProcess.OrderLocation.GreenHouseId == greenHouse)
        {
            int seedTraysWithoutDelivery =
                blockInProcess.SeedTrayAmount - blockInProcess.DeliveryDetails.Sum(x => x.SeedTrayAmountDelivered);

            if (seedTraysWithoutDelivery == relocatedSeedTrays)
            {
                return MovementType.CompleteInAHouse;
            }
            else if (seedTraysWithoutDelivery > relocatedSeedTrays)
            {
                return MovementType.PartialInAHouse;
            }
        }
        else
        {
            Order order = blockInProcess.OrderLocation.Order;

            bool isThereBrother = order.OrderLocations
                .Any(x =>
                    x.GreenHouseId == greenHouse
                    && x.SeedTrayId == blockInProcess.OrderLocation.SeedTrayId
                    && x.RealSowDate == blockInProcess.OrderLocation.RealSowDate
                );

            if (isThereBrother == true)
            {
                return MovementType.WithBortherOutAHouse;
            }
            else
            {
                return MovementType.WithoutBortherOutAHouse;
            }
        }

        throw new Exception("Tipo de movimiento no determinado");
    }

    private void ValidateRelocateChanges(Block blockInProcess, short relocatedSeedTrays)
    {
        int seedTraysWithoutDelivery =
            blockInProcess.SeedTrayAmount - blockInProcess.DeliveryDetails.Sum(x => x.SeedTrayAmountDelivered);

        if (relocatedSeedTrays <= 0 || relocatedSeedTrays > seedTraysWithoutDelivery)
        {
            throw new ArgumentException("La cantidad de bandejas reubicadas debe estar entre 0 " +
                "y la cantidad de bandejas sin entregar de la locación", "relocatedSeedTrays");
        }
    }

    private enum MovementType
    {
        CompleteInAHouse,
        PartialInAHouse,
        WithBortherOutAHouse,
        WithoutBortherOutAHouse,
    };
}
