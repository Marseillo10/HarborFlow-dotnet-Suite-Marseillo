# Test Plan and Execution Log (TEST.md)

This file documents the test plan and the execution of the tests for the HarborFlow WPF project.

## Test Plan

1.  **Run unit tests**: Execute all the unit tests in the `HarborFlow.Tests` project and check for any regressions.

## Test Execution

**2025-10-24 12:00:00** - Running unit tests.

```
dotnet test "/Users/marseillosatrian/Downloads/HarborFflow_WPF/HarborFlow.Tests/HarborFlow.Tests.csproj"
```

**Result:** Failure

**Output:**

```
You must install or update .NET to run this application.
Framework: 'Microsoft.WindowsDesktop.App', version '9.0.0' (arm64)
.NET location: /usr/local/share/dotnet/
No frameworks were found.
```

## Test Summary

The test run failed because the .NET Desktop Runtime is not installed. The user has been instructed to install the missing framework.