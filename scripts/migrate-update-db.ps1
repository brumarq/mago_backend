# Check if migration name was provided
param string $MigrationName

# List of solution directories
$solutions = @(".\DeviceMS", ".\FirmwareMS", ".\MetricsMS", ".\NotificationMS", ".\UserMS")

# Iterate through each solution directory
foreach ($solution in $solutions) {
    Write-Host "Processing $solution"
    $currentDir = Get-Location
    try {
        Set-Location $solution
    } catch {
        Write-Host "Failed to change directory to $solution. Exiting."
        exit 1
    }

    # Define project and startup project names
    $ProjectName = "Infrastructure"
    $StartupProjectName = "WebApp"

    # Run migration
    Write-Host "Running migration '$MigrationName' for $ProjectName"
    dotnet ef migrations add $MigrationName --project $ProjectName --startup-project $StartupProjectName

    if (Test-Path "Migrations\$MigrationName") {
        # Update database
        Write-Host "Updating database for $ProjectName"
        dotnet ef database update --project $ProjectName --startup-project $StartupProjectName
    } else {
        Write-Host "No changes to database model detected. Skipping database update for $ProjectName"
    }

    # Go back to the original directory
    Set-Location $currentDir
}

Write-Host "Migration and update process completed"