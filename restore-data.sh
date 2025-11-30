#!/bin/bash

# Configuration
CONTAINER_NAME="harborflow-db"
DB_USER="postgres"
DB_NAME="harborflow"
INPUT_FILE="harborflow_backup.sql"

if [ ! -f "$INPUT_FILE" ]; then
    echo "Error: Backup file '$INPUT_FILE' not found."
    echo "Please place the .sql file in this directory."
    exit 1
fi

echo "Restoring '$INPUT_FILE' to Docker database '$DB_NAME'..."
echo "WARNING: This will OVERWRITE existing data in the Docker database."
read -p "Are you sure? (y/n) " -n 1 -r
echo ""
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    exit 1
fi

# Check if container is running
if ! docker ps | grep -q $CONTAINER_NAME; then
    echo "Error: Docker container '$CONTAINER_NAME' is not running."
    exit 1
fi

# Restore database
cat $INPUT_FILE | docker exec -i $CONTAINER_NAME psql -U $DB_USER $DB_NAME

if [ $? -eq 0 ]; then
    echo "Restore successful!"
else
    echo "Restore failed."
    exit 1
fi
