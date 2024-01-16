using SupportLayer.Models;

namespace Domain;

public interface IOrderLocationProcessor
{
    IEnumerable<OrderLocation> GetOrderLocationsFromADateOn(DateOnly date);
}