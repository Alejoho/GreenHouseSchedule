using SupportLayer.Models;
using System;

namespace Presentation.IRequesters;

public interface IOrderLocationChangeRequester
{
    void SetTheSownOrderLocation(DateOnly date, int value);
    OrderLocation OrderLocationInProcess { get;}
}
