using SupportLayer.Models;

namespace Presentation.IRequesters;

public interface IClientRequester
{
    void ClientComplete(Client model);
}
