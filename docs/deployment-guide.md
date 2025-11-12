# Deployment Guide

This document provides instructions for deploying the application.

## Deployment Strategy

The application is designed to be deployed as a set of services. The `HarborFlowSuite.Server` project is the backend API, and the `HarborFlowSuite.Client` project is the Blazor WebAssembly frontend.

## Deployment Steps

1.  **Build the application:**
    ```bash
    dotnet publish -c Release
    ```
2.  **Deploy the backend:**
    The published output for the `HarborFlowSuite.Server` project can be deployed to any hosting provider that supports ASP.NET Core, such as Azure App Service, AWS Elastic Beanstalk, or a traditional web server.
3.  **Deploy the frontend:**
    The published output for the `HarborFlowSuite.Client` project is a set of static files that can be deployed to any static web hosting provider, such as GitHub Pages, Netlify, or Vercel.

## CI/CD

No CI/CD pipeline configuration was found in the repository.
