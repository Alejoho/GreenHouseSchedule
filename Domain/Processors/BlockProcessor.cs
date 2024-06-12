﻿using DataAccess.Repositories;
using SupportLayer.Models;

namespace Domain.Processors
{
    //NEXT - change in the database the action ON DELETE from CASCADE to RESTRICT. To avoid delete a record with 
    //dependent records attached to it.
    public class BlockProcessor
    {
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
            }
            else
            {
                _repository.Update(blockInProcess);
            }
        }


        private void UpdateBlockPlaceOutAHouseWithBrother(Block blockInProcess, byte greenHouse, byte block, short relocatedSeedTrays)
        {
            //LATER - remover estos comentarios

            //consiguo el orderlocation brother y el original
            OrderLocation orderLocationBrother = GetOrderLocationsBrother(blockInProcess.OrderLocation, greenHouse);
            OrderLocation orderLocationOriginal = blockInProcess.OrderLocation;

            //actualizar las bandejas y las posturas en el brother
            int alveolus = orderLocationBrother.SeedlingAmount / orderLocationBrother.SeedTrayAmount;
            orderLocationBrother.SeedTrayAmount += relocatedSeedTrays;
            orderLocationBrother.SeedlingAmount += relocatedSeedTrays * alveolus;

            //actualizo el brother en la DB
            OrderLocationRepository orderLocationRepository = new OrderLocationRepository();

            orderLocationRepository.Update(orderLocationBrother);

            //creo el nuebo bloque y lo inserto en la DB
            Block newBlock = new Block()
            {
                OrderLocationId = orderLocationBrother.Id,
                BlockNumber = block,
                SeedTrayAmount = relocatedSeedTrays
            };

            _repository.Insert(newBlock);

            //actualizo el bloque original guardo cambios en la DB
            blockInProcess.SeedTrayAmount -= relocatedSeedTrays;

            if (blockInProcess.SeedTrayAmount == 0)
            {
                _repository.Remove(blockInProcess.Id);
            }
            else
            {
                _repository.Update(blockInProcess);
            }

            //actualizo el orderLocationOriginal en la DB
            orderLocationOriginal.SeedTrayAmount -= relocatedSeedTrays;
            orderLocationOriginal.SeedlingAmount -= relocatedSeedTrays * alveolus;

            if (orderLocationOriginal.SeedTrayAmount == 0)
            {
                orderLocationRepository.Remove(orderLocationOriginal.Id);
            }
            else
            {
                orderLocationRepository.Update(orderLocationOriginal);
            }
        }

        private void UpdateBlockPlaceOutAHouseWithOutBrother(Block blockInProcess, byte greenHouse, byte block, short relocatedSeedTrays)
        {
            OrderLocation orderLocationCopy = GetCopyOfAnOrderLocation(blockInProcess.OrderLocation);
            OrderLocation orderLocationOriginal = blockInProcess.OrderLocation;

            orderLocationCopy.GreenHouseId = greenHouse;
            int alveolus = blockInProcess.OrderLocation.SeedlingAmount / blockInProcess.OrderLocation.SeedTrayAmount;
            orderLocationCopy.SeedTrayAmount = relocatedSeedTrays;
            orderLocationCopy.SeedlingAmount = relocatedSeedTrays * alveolus;



            Block newBlock = new Block()
            {
                OrderLocationId = orderLocationCopy.Id,
                BlockNumber = block,
                SeedTrayAmount = relocatedSeedTrays
            };

            _repository.Insert(newBlock);

            blockInProcess.SeedTrayAmount -= relocatedSeedTrays;

            if (blockInProcess.SeedTrayAmount == 0)
            {
                _repository.Remove(blockInProcess.Id);
            }
            else
            {
                _repository.Update(blockInProcess);
            }
            throw new NotImplementedException();
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

        private OrderLocation GetOrderLocationsBrother(OrderLocation orderLocation, byte greenHouse)
        {
            Order order = orderLocation.Order;

            var output = order.OrderLocations.Where(x =>
                    x.GreenHouseId == greenHouse
                    && x.SeedTrayId == orderLocation.SeedTrayId
                    && x.RealSowDate == orderLocation.RealSowDate);

            if (output.Count() > 1)
            {
                throw new ApplicationException("There's more than 1 order location brother.");
            }

            if (output.Count() == 0)
            {
                throw new ApplicationException("There's no order location brother.");
            }

            return output.First();
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
            MovementType output;

            if (blockInProcess.OrderLocation.GreenHouseId == greenHouse)
            {
                int seedTraysWithoutDelivery =
                    blockInProcess.SeedTrayAmount - blockInProcess.DeliveryDetails.Sum(x => x.SeedTrayAmountDelivered);

                if (seedTraysWithoutDelivery == relocatedSeedTrays)
                {
                    output = MovementType.CompleteInAHouse;
                }
                else if (seedTraysWithoutDelivery > relocatedSeedTrays)
                {
                    output = MovementType.PartialInAHouse;
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
}
