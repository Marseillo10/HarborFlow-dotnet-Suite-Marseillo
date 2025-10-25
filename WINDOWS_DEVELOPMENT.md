# Windows-Specific Development Tasks for HarborFlow

This document outlines development tasks that are specific to the Windows environment due to the project's use of Windows Presentation Foundation (WPF).

## Background

While the majority of the codebase (.NET Class Libraries for Core, Application, and Infrastructure) is cross-platform, the user interface (`HarborFlow.Wpf`) is a WPF application. This creates some limitations for developers not using Windows.

## Why Windows is Required for Full Testing

### 1. Running and Debugging the UI

The main `HarborFlow.Wpf` application can **only** be run and debugged on a Windows operating system. This is the only way to visually verify that the UI, including all new features and layouts, functions as expected.

**Steps on Windows:**
- Open the solution file (`HarborFlow.sln`) in Visual Studio.
- Set `HarborFlow.Wpf` as the "Startup Project".
- Run the application by pressing `F5` or the "Start" button.

### 2. Running Unit & UI Tests

Executing the tests in the `HarborFlow.Tests` project requires the .NET Desktop Runtime (`Microsoft.WindowsDesktop.App`), which is only available on Windows. Running these tests is critical for validating ViewModel logic and preventing regressions.

**Steps on Windows:**
- Open a terminal (Command Prompt or PowerShell) in the project's root directory.
- Run the command:
  ```shell
  dotnet test
  ```
- Alternatively, run the tests via the **Test Explorer** in Visual Studio.

### 3. Verifying API Integrations

Since the application's UI can only be run on Windows, verifying that new API integrations are working correctly is also a Windows-specific task. For example, to verify the Global Fishing Watch API:

1.  **Jalankan Aplikasi**: Buka terminal di direktori proyek dan jalankan perintah:
    ```shell
    dotnet run --project HarborFlow.Wpf
    ```
2.  **Amati Konsol**: Perhatikan jendela terminal tempat Anda menjalankan perintah tersebut. Di sinilah log aplikasi akan muncul.
3.  **Cari Detail Kapal**: Di dalam aplikasi, lakukan aksi yang akan memicu pencarian detail kapal (misalnya, mengklik kapal di peta atau menggunakan fitur pencarian).
4.  **Periksa Log**:
    *   Jika berhasil, Anda akan melihat detail kapal (nama, tipe, dll.) muncul di UI.
    *   Jika gagal (misalnya, kunci API salah atau tidak ditemukan), sebuah pesan *error* atau *warning* akan tercetak di terminal, seperti `Global Fishing Watch API key is not configured` atau `HTTP request to Global Fishing Watch API failed`.

## Cross-Platform Contributions (macOS/Linux)

Developers on non-Windows systems can fully contribute to the core logic of the application. The setup process for the database is now seamless on any platform thanks to Docker.

**What you CAN do on macOS/Linux:**
- **Set up the database:** Run `docker-compose up -d` and `dotnet ef database update`.
- **Build the solution:** Run `dotnet build` to check for compilation errors in the core projects.
- **Develop features:** Write and modify code in the `HarborFlow.Core`, `HarborFlow.Application`, and `HarborFlow.Infrastructure` projects.

**What you CANNOT do on macOS/Linux:**
- Run the `HarborFlow.Wpf` application to see the UI.
- Run the tests in the `HarborFlow.Tests` project.

## Conclusion

Any work that touches the UI (`HarborFlow.Wpf`) or requires validation through tests (`HarborFlow.Tests`) **must** ultimately be validated on a Windows environment before being considered complete. However, the core backend development is fully cross-platform.