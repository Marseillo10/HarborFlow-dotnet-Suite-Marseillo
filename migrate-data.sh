#!/bin/bash

echo "HarborFlow Data Migration Tool"
echo "=============================="
echo "This script will copy data from your LOCAL Postgres (port 5432) to the DOCKER Postgres (port 5433)."
echo "WARNING: This will OVERWRITE data in the Docker database."
echo ""

# Source Credentials (from user input)
SRC_HOST="localhost"
SRC_PORT="5432"
SRC_DB="harborflowdb"
SRC_USER="marseillosatrian"
# export PGPASSWORD="bizero11" (Used inline below)

# Target Credentials (Docker)
TGT_HOST="localhost"
TGT_PORT="5433"
TGT_DB="harborflow"
TGT_USER="postgres"
TGT_PASS="password"

echo "Source: $SRC_DB on port $SRC_PORT (User: $SRC_USER)"
echo "Target: $TGT_DB on port $TGT_PORT (User: $TGT_USER)"
echo ""
read -p "Are you sure you want to proceed? (y/n) " -n 1 -r
echo ""
if [[ ! $REPLY =~ ^[Yy]$ ]]
then
    echo "Migration cancelled."
    exit 1
fi

echo "Step 1: Dumping local database..."

# Check if source DB exists and is accessible
if ! PGPASSWORD="bizero11" psql -h $SRC_HOST -p $SRC_PORT -U $SRC_USER -d $SRC_DB -c '\q' 2>/dev/null; then
    echo "Error: Cannot connect to source database. Please check if your local Postgres is running on port 5432."
    exit 1
fi

# Dump data only (to avoid schema conflicts, assuming app already created schema)
# Or full dump? Let's try data only first to be safe with existing migrations.
# Actually, if the Docker DB is fresh, a full dump (clean) might be better, 
# BUT the target DB name is different (harborflow vs harborflowdb).
# So we dump content and pipe to target.

echo "Step 2: Restoring to Docker database..."
PGPASSWORD="bizero11" pg_dump -h $SRC_HOST -p $SRC_PORT -U $SRC_USER --data-only --column-inserts $SRC_DB | \
PGPASSWORD="password" psql -h $TGT_HOST -p $TGT_PORT -U $TGT_USER $TGT_DB

echo ""
echo "Migration completed (check for any errors above)."
