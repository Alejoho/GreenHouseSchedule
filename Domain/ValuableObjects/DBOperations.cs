using DataAccess;

namespace Domain.ValuableObjects;

public static class DBOperations
{
    public static async Task BackupDatabaseAsync()
    {
        await DataBaseOperations.BackupDatabaseAsync();
    }
}
