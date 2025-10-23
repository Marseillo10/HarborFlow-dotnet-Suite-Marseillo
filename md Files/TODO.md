# Daftar Tugas Pengembangan HarborFlow WPF

Berikut adalah rincian tugas berdasarkan rencana pengembangan yang telah disetujui.

### Fase 1: Implementasi ViewModel dan Logika Bisnis

-   [ ] `[viewmodel]` Buat file `LoginViewModel.cs`.
-   [ ] `[viewmodel]` Implementasikan properti untuk `Username`, `Password`, dan `IsLoading` di `LoginViewModel`.
-   [ ] `[viewmodel]` Implementasikan `LoginCommand` di `LoginViewModel` yang memanggil `IAuthService.LoginAsync`.
-   [ ] `[viewmodel]` Implementasikan navigasi ke `MainViewModel` setelah login berhasil.
-   [ ] `[viewmodel]` Buat file `MainViewModel.cs`.
-   [ ] `[viewmodel]` Implementasikan properti `CurrentViewModel` dan logika navigasi di `MainViewModel`.
-   [ ] `[viewmodel]` Implementasikan `LogoutCommand` di `MainViewModel`.
-   [ ] `[viewmodel]` Buat file `DashboardViewModel.cs` dan implementasikan logika pemuatan data ringkasan. (Dapat berjalan paralel)
-   [ ] `[viewmodel]` Buat file `VesselManagementViewModel.cs`.
-   [ ] `[viewmodel]` Implementasikan `ObservableCollection<Vessel>` dan `SelectedVessel` di `VesselManagementViewModel`.
-   [ ] `[viewmodel]` Implementasikan `ICommand` untuk operasi CRUD (Create, Read, Update, Delete) kapal di `VesselManagementViewModel`. (Dapat berjalan paralel)
-   [ ] `[viewmodel]` Buat file `ServiceRequestViewModel.cs` dan implementasikan logika untuk memuat dan mengelola permintaan layanan. (Dapat berjalan paralel)

### Fase 2: Pengembangan View (XAML)

-   [ ] `[view]` Buat atau perbarui `LoginView.xaml` dan ikat ke `LoginViewModel`.
-   [ ] `[view]` Perbarui `MainWindow.xaml` untuk menangani tata letak utama setelah login, termasuk `ContentControl` untuk view dinamis.
-   [ ] `[view]` Buat `DashboardView.xaml` dan ikat ke `DashboardViewModel`. (Dapat berjalan paralel)
-   [ ] `[view]` Buat `VesselManagementView.xaml` dan ikat ke `VesselManagementViewModel`. (Dapat berjalan paralel)
-   [ ] `[view]` Buat `ServiceRequestView.xaml` dan ikat ke `ServiceRequestViewModel`. (Dapat berjalan paralel)

### Fase 3: Integrasi Layanan

-   [ ] `[integration]` Konfigurasikan Dependency Injection di `App.xaml.cs` untuk semua layanan dan viewmodel.
-   [ ] `[integration]` Panggil metode layanan dari `LoginViewModel` dan kelola hasilnya.
-   [ ] `[integration]` Panggil metode layanan dari `DashboardViewModel`.
-   [ ] `[integration]` Panggil metode layanan dari `VesselManagementViewModel`.
-   [ ] `[integration]` Panggil metode layanan dari `ServiceRequestViewModel`.
-   [ ] `[integration]` Implementasikan penanganan kesalahan dan status `loading` di semua viewmodel yang relevan.

### Fase 4: Pengujian Unit

-   [ ] `[test]` Siapkan proyek pengujian `HarborFlow.Tests` dengan pustaka mocking (misalnya, Moq).
-   [ ] `[test]` Tulis pengujian unit untuk `LoginViewModel`.
-   [ ] `[test]` Tulis pengujian unit untuk `MainViewModel`.
-   [ ] `[test]` Tulis pengujian unit untuk `VesselManagementViewModel`.
-   [ ] `[test]` Tulis pengujian unit untuk layanan aplikasi jika belum ada.

### Fase 5: Pengujian Integrasi dan Pengguna

-   [ ] `[test]` Lakukan pengujian manual untuk alur kerja login dan logout.
-   [ ] `[test]` Lakukan pengujian manual untuk alur kerja manajemen kapal (CRUD).
-   [ ] `[test]` Lakukan pengujian manual untuk alur kerja permintaan layanan.
-   [ ] `[test]` Verifikasi penanganan kesalahan UI untuk skenario seperti login yang gagal.