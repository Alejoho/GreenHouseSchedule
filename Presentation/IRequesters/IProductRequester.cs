using SupportLayer.Models;

namespace Presentation.IRequesters;

public interface IProductRequester
{
    void ProductComplete(Product model);
}
