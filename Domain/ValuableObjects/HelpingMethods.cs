using SupportLayer.Models;

namespace Domain.ValuableObjects;

internal static class HelpingMethods
{
    internal static OrderLocation GetCopyOfAnOrderLocation(OrderLocation orderLocation)
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

    internal static OrderLocation GetOrderLocationsBrother(OrderLocation orderLocation, byte greenHouse)
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
}
