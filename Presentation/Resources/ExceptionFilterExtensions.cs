using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Resources;

public static class ExceptionFilterExtensions
{
    public static bool IsForeignKeyViolation(this DbUpdateException ex)
        => ex.InnerException is SqlException sqlEx && sqlEx.Number == 547;
}
