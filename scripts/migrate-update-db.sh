#!/bin/bash

# Check if migration name was provided
if [ "$#" -ne 1 ]; then
  echo "No migration name provided. Usage: $0 <MigrationName>"
  exit 1
fi

MIGRATION_NAME="$1"

# current_dir=$(pwd)

# List of solution directories
declare -a solutions=("DeviceMS" "FirmwareMS" "MetricsMS" "NotificationMS" "UserMS")

# Iterate through each solution directory
for solution in "${solutions[@]}"
do
  echo "Processing $solution"
  cd "$solution" || { echo "Failed to change directory to $solution. Exiting."; exit 1; }
  echo "Changed directory to $(pwd)"
  
  # Define project and startup project names
  PROJECT_NAME="Infrastructure"
  STARTUP_PROJECT_NAME="WebApp"
  
  # Run migration
  echo "Running migration '$MIGRATION_NAME' for '$PROJECT_NAME'"
  dotnet ef migrations add "$MIGRATION_NAME" --project $PROJECT_NAME --startup-project $STARTUP_PROJECT_NAME
  echo "Created file?"
  
 # count=0
 # while [ ! -d "Migrations/$MIGRATION_NAME" ] && [ $count -lt 10 ]; do
 #   sleep 1
 #   ((count++))
 # done
  
  if [ -d "Migrations/${MIGRATION_NAME}" ]; then
    # Update database
    echo "Updating database for '$PROJECT_NAME'"
    dotnet ef database update --project $PROJECT_NAME --startup-project $STARTUP_PROJECT_NAME
  else
    echo "No changes to database model detected. Skipping database update for $PROJECT_NAME"
  fi
  
  # Go back to the original directory
  cd - || cd ..
done

echo "Migration and update process completed"