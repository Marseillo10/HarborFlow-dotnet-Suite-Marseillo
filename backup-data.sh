#!/bin/bash

# Configuration
CONTAINER_NAME="harborflow-db"
DB_USER="postgres"
DB_NAME="harborflow"
OUTPUT_FILE="harborflow_backup.sql"

echo "Backing up Docker database '$DB_NAME' to '$OUTPUT_FILE'..."

# Check if container is running
if ! docker ps | grep -q $CONTAINER_NAME; then
    echo "Error: Docker container '$CONTAINER_NAME' is not running."
    exit 1
fi

# Dump database
docker exec -t $CONTAINER_NAME pg_dump -U $DB_USER -c $DB_NAME > $OUTPUT_FILE

if [ $? -eq 0 ]; then
    echo "Backup successful! File saved as: $OUTPUT_FILE"
    echo "You can share this file with other developers."
else
    echo "Backup failed."
    exit 1
fi
