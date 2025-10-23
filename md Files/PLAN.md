# Rencana Pengembangan Proyek HarborFlow WPF - Fase 2

Rencana ini melanjutkan pengembangan aplikasi HarborFlow WPF, berfokus pada penyempurnaan fitur yang ada dan implementasi logika bisnis yang lebih dalam.

## Fase 1: Fungsionalitas Manajemen Permintaan Layanan (Fitur F-003)

Tujuan dari fase ini adalah untuk melengkapi alur kerja manajemen permintaan layanan.

1.  **Implementasi Alur Persetujuan:**
    *   `[viewmodel]` Tambahkan `ApproveCommand` dan `RejectCommand` ke `ServiceRequestViewModel`.
    *   `[viewmodel]` `CanExecute` untuk command ini harus memeriksa apakah sebuah permintaan dipilih dan apakah pengguna memiliki peran yang sesuai (Petugas Pelabuhan/Admin).
    *   `[viewmodel]` Panggil metode `ApproveServiceRequestAsync` dan `RejectServiceRequestAsync` dari `IPortServiceManager`.
    *   `[view]` Tambahkan tombol "Approve" dan "Reject" ke `ServiceRequestView.xaml`.
    *   `[view]` Ikat visibilitas tombol-tombol ini ke peran pengguna.

2.  **Implementasi Fungsionalitas Hapus:**
    *   `[backend]` Tambahkan metode `DeleteServiceRequestAsync(Guid requestId)` ke `IPortServiceManager.cs`.
    *   `[backend]` Implementasikan `DeleteServiceRequestAsync` di `PortServiceManager.cs` untuk menghapus permintaan dari database.
    *   `[viewmodel]` Implementasikan logika untuk `DeleteServiceRequestCommand` di `ServiceRequestViewModel` untuk memanggil metode layanan yang baru.

## Fase 2: Penyempurnaan Manajemen Kapal (Fitur F-002)

Fase ini berfokus pada peningkatan pengalaman pengguna untuk manajemen kapal.

1.  **UI Dialog yang Disempurnakan:**
    *   `[view]` Tinjau dan perbaiki layout `VesselEditorView.xaml` agar lebih intuitif.
    *   `[viewmodel]` Tambahkan validasi input ke `VesselEditorViewModel` (misalnya, memastikan IMO unik dan memiliki format yang benar).
    *   `[view]` Tampilkan pesan validasi di `VesselEditorView`.

2.  **Notifikasi Pengguna:**
    *   `[viewmodel]` Gunakan `INotificationService` di `VesselManagementViewModel` untuk menampilkan notifikasi setelah operasi tambah, edit, atau hapus berhasil atau gagal.

## Fase 3: Peningkatan Tampilan Peta (Fitur F-001)

Menambahkan fitur yang lebih kaya ke tampilan peta.

1.  **Filter Berdasarkan Tipe Kapal:**
    *   `[viewmodel]` Tambahkan `ObservableCollection<VesselType>` dan properti `SelectedVesselType` ke `MapViewModel`.
    *   `[viewmodel]` Modifikasi logika untuk memfilter koleksi `VesselsOnMap` berdasarkan `SelectedVesselType`.
    *   `[view]` Tambahkan `ComboBox` atau `ListBox` ke `MapView.xaml` untuk menampilkan filter tipe kapal.

2.  **Pencarian Auto-complete:**
    *   `[viewmodel]` Sempurnakan metode `UpdateSuggestionsAsync` di `MapViewModel` untuk memberikan saran yang lebih relevan saat pengguna mengetik.
    *   `[view]` Pastikan UI pencarian menampilkan saran ini dengan benar.

## Fase 4: Implementasi Kontrol Akses Berbasis Peran (RBAC)

Memastikan pengguna hanya dapat melihat dan melakukan aksi yang sesuai dengan peran mereka.

1.  **Visibilitas UI:**
    *   `[viewmodel]` Tambahkan properti boolean ke ViewModel yang relevan (misalnya, `CanEditVessels`, `CanApproveRequests`) yang didasarkan pada `SessionContext.CurrentUser.Role`.
    *   `[view]` Ikat properti `Visibility` atau `IsEnabled` dari kontrol UI (tombol, item menu) ke properti boolean ini di ViewModel.

## Fase 5: Poles UI/UX

Penyempurnaan akhir untuk meningkatkan pengalaman pengguna secara keseluruhan.

1.  **Desain Ulang Dashboard:**
    *   `[view]` Tata ulang `DashboardView.xaml` menggunakan `Grid`, `Border`, dan `ItemsControl` untuk membuat tata letak berbasis kartu yang lebih modern.
2.  **Indikator Loading:**
    *   `[view]` Pastikan semua view yang melakukan operasi data yang berjalan lama menampilkan indikator loading (misalnya, `ProgressBar`) yang terikat pada properti `IsLoading` di ViewModel masing-masing.
