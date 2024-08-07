﻿using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.ValuableObjects;
using log4net;
using SupportLayer;
using SupportLayer.Models;

namespace Domain.Processors;


public class BlockProcessor
{
    private ILog _log;
    private IBlockRepository _blockRepository;
    private IOrderLocationRepository _orderLocationRepository;
    private IGreenHouseRepository _greenHouseRepository;

    public BlockProcessor()
    {
        _blockRepository = new BlockRepository();
        _orderLocationRepository = new OrderLocationRepository();
        _greenHouseRepository = new GreenHouseRepository();
        _log = LogHelper.GetLogger();
    }

    public BlockProcessor(ILog log
        , IBlockRepository blockRepository
        , IOrderLocationRepository orderLocationRepository
        , IGreenHouseRepository greenHouseRepository)
    {
        _blockRepository = blockRepository;
        _orderLocationRepository = orderLocationRepository;
        _greenHouseRepository = greenHouseRepository;
        _log = log;
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

        _blockRepository.Insert(newBlock);

        blockInProcess.SeedTrayAmount -= relocatedSeedTrays;

        if (blockInProcess.SeedTrayAmount == 0)
        {
            _blockRepository.Remove(blockInProcess.Id);
            blockInProcess.OrderLocation.Blocks.Remove(blockInProcess);
        }
        else
        {
            _blockRepository.Update(blockInProcess);
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

        OrderLocation sender = blockInProcess.OrderLocation;

        int alveolus = sender.SeedlingAmount / sender.SeedTrayAmount;
        reciever.SeedTrayAmount += relocatedSeedTrays;
        reciever.SeedlingAmount += relocatedSeedTrays * alveolus;

        _orderLocationRepository.Update(reciever);

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

        _blockRepository.Insert(newBlock);

        blockInProcess.SeedTrayAmount -= relocatedSeedTrays;

        if (blockInProcess.SeedTrayAmount == 0)
        {
            _blockRepository.Remove(blockInProcess.Id);
            sender.Blocks.Remove(blockInProcess);
        }
        else
        {
            _blockRepository.Update(blockInProcess);
        }

        sender.SeedTrayAmount -= relocatedSeedTrays;
        sender.SeedlingAmount -= relocatedSeedTrays * alveolus;

        if (sender.SeedTrayAmount == 0)
        {
            _orderLocationRepository.Remove(sender.Id);
            sender.Order.OrderLocations.Remove(sender);
        }
        else
        {
            _orderLocationRepository.Update(sender);
        }

        reciever.Blocks.Add(newBlock);

        //I can assign here the house
        newBlock.OrderLocation = reciever;
    }

    private void UpdateBlockPlaceOutAHouseWithBrother(Block blockInProcess, byte greenHouse, byte block, short relocatedSeedTrays)
    {
        OrderLocation orderLocationBrother = HelpingMethods.GetOrderLocationsBrother(blockInProcess.OrderLocation, greenHouse);

        TransferBlock(orderLocationBrother, blockInProcess, block, relocatedSeedTrays);

        _log.Info("Completed the relocation of the block. UpdateBlockPlaceOutAHouseWithBrother");
    }

    private void UpdateBlockPlaceOutAHouseWithOutBrother(Block blockInProcess, byte greenHouse, byte block, short relocatedSeedTrays)
    {
        OrderLocation orderLocationCopy = HelpingMethods.GetCopyOfAnOrderLocation(blockInProcess.OrderLocation);

        orderLocationCopy.GreenHouseId = greenHouse;
        orderLocationCopy.SeedTrayAmount = 0;
        orderLocationCopy.SeedlingAmount = 0;

        _orderLocationRepository.Insert(orderLocationCopy);

        TransferBlock(orderLocationCopy, blockInProcess, block, relocatedSeedTrays);

        orderLocationCopy.SeedTray = blockInProcess.OrderLocation.SeedTray;
        orderLocationCopy.Order = blockInProcess.OrderLocation.Order;

        orderLocationCopy.GreenHouse = _greenHouseRepository.GetAll().First(x => x.Id == greenHouse);

        blockInProcess.OrderLocation.Order.OrderLocations.Add(orderLocationCopy);

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
                "y la cantidad de bandejas sin entregar de la locación", nameof(relocatedSeedTrays));
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
