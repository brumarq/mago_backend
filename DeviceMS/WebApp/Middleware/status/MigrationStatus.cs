namespace WebApp.Middleware.status;

public class MigrationStatus
{
    public bool IsMigrationSuccessful { get; set; }

    public void SetMigrationStatus(bool success)
    {
        IsMigrationSuccessful = success;
    }
}