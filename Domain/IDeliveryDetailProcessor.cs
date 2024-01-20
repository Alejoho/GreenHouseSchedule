using SupportLayer.Models;

namespace Domain
{
    public interface IDeliveryDetailProcessor
    {
        IEnumerable<DeliveryDetail> GetDeliveryDetailFromADateOn(DateOnly date);
    }
}