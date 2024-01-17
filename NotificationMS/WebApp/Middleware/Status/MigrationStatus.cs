namespace WebApp.Middleware.Status;

public class MigrationStatus
{
    public bool IsMigrationSuccessful { get; set; }

    public void SetMigrationStatus(bool success)
    {
        IsMigrationSuccessful = success;
    }
}