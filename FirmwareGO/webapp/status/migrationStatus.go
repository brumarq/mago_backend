package status

var migrationSuccessful bool

// SetMigrationStatus sets the status of the migration.
func SetMigrationStatus(success bool) {
	migrationSuccessful = success
}

// IsMigrationSuccessful returns the current status of the migration.
func IsMigrationSuccessful() bool {
	return migrationSuccessful
}
