using SupportLayer.Models;

namespace Presentation.IRequesters;

public interface IRelocatedBlockRequester
{
    void SetTheRelocatedBlock(byte greenHouse, byte block, short relocatedSeedTrays);
    Block BlockInProcess { get; }
}
