using SupportLayer.Models;
using System;

namespace Presentation.IRequesters;

public interface IDeliveredBlockRequester
{
    void SetTheDeliveredBlock(DateOnly date, short deliveredSeedTrays);
    Block BlockInProcess { get; }
}
