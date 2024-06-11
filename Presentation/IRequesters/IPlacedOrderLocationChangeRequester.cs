using SupportLayer.Models;

namespace Presentation.IRequesters
{
    public interface IPlacedOrderLocationChangeRequester
    {
        void SetThePlacedOrderLocation(byte greenHouse, byte block, short placedSeedTrays);
        OrderLocation OrderLocationInProcess { get; }
    }
}
