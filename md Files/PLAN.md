# Rencana Pengembangan Proyek HarborFlow WPF

Rencana ini menguraikan langkah-langkah untuk menyelesaikan pengembangan aplikasi desktop HarborFlow WPF berdasarkan spesifikasi teknis dan kemajuan saat ini.

## Fase 1: Implementasi ViewModel dan Logika Bisnis

Tujuan dari fase ini adalah untuk membuat semua ViewModels yang diperlukan dan mengimplementasikan logika bisnis inti di dalamnya.

1.  **LoginViewModel:**
    *   Properti untuk `Username`, `Password`.
    *   `ICommand` untuk proses login (`LoginCommand`).
    *   Logika untuk memanggil `IAuthService.LoginAsync`.
    *   Manajemen status (misalnya, `IsLoading`).
    *   Navigasi ke `MainViewModel` setelah login berhasil.

2.  **MainViewModel:**
    *   Properti untuk menampung `ViewModel` aktif saat ini (misalnya, `CurrentViewModel`).
    *   `ICommand` untuk navigasi antar-view (mis. `NavigateToVesselsCommand`, `NavigateToDashboardCommand`).
    *   Logika untuk logout.

3.  **DashboardViewModel:**
    *   Properti untuk menampilkan data ringkasan (misalnya, jumlah kapal, permintaan layanan yang aktif).
    *   Logika untuk memuat data dari layanan yang relevan.

4.  **VesselManagementViewModel:**
    *   `ObservableCollection<Vessel>` untuk menampung daftar kapal.
    *   `ICommand` untuk menambah, mengedit, dan menghapus kapal.
    *   Logika untuk memanggil layanan terkait kapal dari `IPortServiceManager`.
    *   Properti untuk kapal yang dipilih (`SelectedVessel`).

5.  **ServiceRequestViewModel:**
    *   `ObservableCollection<ServiceRequest>` untuk daftar permintaan layanan.
    *   `ICommand` untuk membuat dan memperbarui permintaan layanan.
    *   Logika untuk memuat data permintaan layanan.

## Fase 2: Pengembangan View (XAML)

Fase ini berfokus pada pembuatan antarmuka pengguna dan mengikatnya ke ViewModels yang telah dibuat.

1.  **LoginView.xaml:**
    *   Input untuk `Username` dan `Password`, diikat ke `LoginViewModel`.
    *   Tombol Login, diikat ke `LoginCommand`.
    *   Indikator loading.

2.  **MainWindow.xaml (setelah login):**
    *   Struktur layout utama dengan area navigasi dan area konten.
    *   `ContentControl` yang diikat ke `MainViewModel.CurrentViewModel` untuk menampilkan view aktif.
    *   Menu navigasi yang diikat ke `ICommand` navigasi di `MainViewModel`.

3.  **DashboardView.xaml:**
    *   Menampilkan data ringkasan dari `DashboardViewModel`.
    *   Menggunakan bagan atau panel info.

4.  **VesselManagementView.xaml:**
    *   `DataGrid` atau `ListView` untuk menampilkan daftar kapal dari `VesselManagementViewModel`.
    *   Formulir untuk menambah/mengedit kapal, diikat ke properti di `VesselManagementViewModel`.
    *   Tombol untuk aksi (Tambah, Edit, Hapus).

5.  **ServiceRequestView.xaml:**
    *   `DataGrid` atau `ListView` untuk menampilkan permintaan layanan.
    *   Formulir untuk membuat/mengelola permintaan.

## Fase 3: Integrasi Layanan

Menghubungkan ViewModels dengan layanan backend untuk fungsionalitas penuh.

1.  **Injeksi Dependensi:** Konfigurasikan `App.xaml.cs` untuk menginjeksi `IAuthService`, `IPortServiceManager`, dan layanan lain ke dalam ViewModels yang sesuai.
2.  **Implementasi Panggilan Layanan:** Panggil metode layanan (misalnya, `LoginAsync`, `GetAllVesselsAsync`) dari dalam ViewModels dan kelola hasilnya (misalnya, memperbarui `ObservableCollection`, menangani kesalahan).
3.  **Manajemen State:** Implementasikan manajemen state yang kuat, seperti menampilkan pesan kesalahan kepada pengguna atau menampilkan indikator loading selama operasi yang berjalan lama.

## Fase 4: Pengujian Unit

Memastikan setiap komponen bekerja secara terpisah seperti yang diharapkan.

1.  **Test Proyek:** Buat proyek pengujian baru jika belum ada.
2.  **ViewModel Tests:** Tulis pengujian unit untuk setiap `ViewModel`:
    *   Verifikasi logika `ICommand`.
    *   Verifikasi perubahan properti.
    *   Gunakan `mock` untuk dependensi layanan untuk mengisolasi logika ViewModel.
3.  **Service Tests:** Tulis pengujian unit untuk layanan di `HarborFlow.Application` jika belum ada.

## Fase 5: Pengujian Integrasi dan Pengguna

Memastikan seluruh aplikasi bekerja sebagai satu kesatuan yang kohesif.

1.  **Pengujian Alur Kerja:** Uji alur kerja utama dari awal hingga akhir:
    *   Login -> Lihat Dashboard -> Kelola Kapal -> Buat Permintaan Layanan -> Logout.
2.  **Pengujian UI:** Verifikasi bahwa semua elemen UI ditampilkan dengan benar dan merespons interaksi pengguna seperti yang diharapkan.
3.  **Penanganan Kesalahan:** Uji skenario kesalahan (misalnya, kredensial login yang salah, kegagalan koneksi jaringan) dan pastikan aplikasi menanganinya dengan baik.