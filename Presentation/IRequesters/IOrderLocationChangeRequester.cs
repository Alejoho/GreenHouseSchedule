using SupportLayer.Models;
using System;

namespace Presentation.IRequesters;

public interface IOrderLocationChangeRequester
{
    void SetTheSownOrderLocation(DateOnly date, short sownSeedTrays);
    OrderLocation OrderLocationInProcess { get;}
}
