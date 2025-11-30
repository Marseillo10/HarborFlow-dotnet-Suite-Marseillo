#!/bin/bash

echo "HarborFlow Development Environment Starter"
echo "=========================================="

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
  echo "Error: Docker is not running."
  echo "Please start Docker Desktop and try again."
  exit 1
fi

echo "Docker is running."

# Check if port 5433 is already in use
if lsof -Pi :5433 -sTCP:LISTEN -t >/dev/null ; then
    echo "Warning: Port 5433 is already in use."
    echo "Checking if it's our container..."
    if docker ps | grep "harborflow_db" > /dev/null; then
        echo "It's harborflow_db. Good."
    else
        echo "Error: Something else is using port 5433. Please free it up."
        exit 1
    fi
fi

echo "Starting Database Container..."
docker-compose up -d

echo "Waiting for Database to be ready..."
# Simple wait loop
until docker exec harborflow_db pg_isready -U postgres > /dev/null 2>&1; do
  echo -n "."
  sleep 1
done
echo ""
echo "Database is ready!"

echo ""
echo "You can now run the application:"
echo "dotnet run --project HarborFlowSuite/HarborFlowSuite.Server --launch-profile https"
