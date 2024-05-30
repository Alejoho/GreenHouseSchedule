using SupportLayer.Models;

namespace Presentation.IRequesters
{
    public interface IPlacedOrderLocationChangeRequester
    {
        void SetThePlacedOrderLocation(int greenHouse, int block, short sownSeedTrays);
        OrderLocation OrderLocationInProcess { get; }
    }
}
