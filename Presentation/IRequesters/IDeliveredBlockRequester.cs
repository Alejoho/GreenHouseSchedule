using SupportLayer.Models;

namespace Presentation.IRequesters;

public interface IDeliveredBlockRequester
{
    void SetTheDeliveredBlock();
    Block BlockInProcess { get; }
}
