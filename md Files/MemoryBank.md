
# Memory Bank: Proyek HarborFlow WPF

## 1. Detail Proyek

- **Nama Proyek**: HarborFlow
- **Deskripsi**: Sistem Manajemen Pelabuhan Cerdas yang dirancang untuk mendigitalkan dan mengoptimalkan alur kerja operasional di pelabuhan. Ini adalah aplikasi desktop yang dibangun dengan .NET 9 dan WPF, menampilkan tema Fluent yang modern.
- **Masalah Bisnis Inti yang Diselesaikan**:
    - Kurangnya digitalisasi (proses manual yang tidak efisien).
    - Fragmentasi informasi di antara para pemangku kepentingan.
    - Pelacakan kapal yang manual dan rawan kesalahan.
- **Pemangku Kepentingan Utama**: Petugas Pelabuhan, Agen Maritim, Operator Kapal, Manajemen Pelabuhan.

## 2. Arsitektur

- **Gaya**: Arsitektur 3-Tier dengan pola Model-View-ViewModel (MVVM).
- **Lapisan**:
    - **Lapisan Presentasi**: Aplikasi WPF (`HarborFlow.Wpf`) yang bertanggung jawab atas UI dan interaksi pengguna.
    - **Lapisan Logika Bisnis**: Pustaka Kelas .NET (`HarborFlow.Application`) yang berisi aturan bisnis, validasi, dan antarmuka layanan.
    - **Lapisan Akses Data**: Pustaka Kelas .NET (`HarborFlow.Infrastructure`) yang bertanggung jawab atas persistensi data, komunikasi dengan database, dan implementasi layanan eksternal.
- **Alur Data**: Alur data searah dari Lapisan Presentasi ke Lapisan Logika Bisnis dan kemudian ke Lapisan Akses Data.
- **Injeksi Dependensi**: Aplikasi menggunakan `Microsoft.Extensions.DependencyInjection` untuk mengelola dependensi antar lapisan.

## 3. Tumpukan Teknologi

- **Framework Utama**: .NET 9
- **Framework UI**: Windows Presentation Foundation (WPF)
- **Database**: PostgreSQL 17
- **ORM**: Entity Framework Core 9 dengan penyedia Npgsql.
- **Validasi**: FluentValidation
- **Bahasa Pemrograman**: C# 12 dan XAML.
- **Pustaka Kunci**:
    - `Microsoft.Extensions.Hosting` untuk hosting dan konfigurasi aplikasi.
    - `Microsoft.Extensions.DependencyInjection` untuk DI.
    - `Microsoft.Web.WebView2` untuk menampilkan konten web (peta).

## 4. Kebutuhan Proyek

### Fitur Wajib Dimiliki:
- **F-001: Peta Dasar**: Peta interaktif yang menampilkan posisi kapal secara real-time menggunakan data dari penyedia AIS.
- **F-002: Pencarian Kapal**: Kemampuan untuk mencari kapal berdasarkan nama atau nomor IMO.
- **F-003: Manajemen Layanan Pelabuhan**: Platform untuk mengajukan dan menyetujui/menolak permintaan layanan, termasuk lampiran dokumen.

### Fitur Sebaiknya Dimiliki:
- Filter tampilan peta berdasarkan jenis kapal.
- Pelengkapan otomatis untuk pencarian nama kapal.

### Kebutuhan Teknis Utama:
- Kompatibilitas dengan .NET 9.
- Arsitektur 3-lapis.
- UI yang responsif.
- Kontrol akses berbasis peran (RBAC).

## 5. Status/Progres Pekerjaan

- **Pengaturan Proyek**: Solusi dan proyek telah diatur dengan DI.
- **Fitur F-001 (Tampilan Peta)**: **Selesai** (dengan data dummy).
    - Migrasi dari Bing Maps ke solusi OpenStreetMap + Leaflet.js menggunakan WebView2 telah selesai.
    - Logika untuk mengambil data dari API (dengan fallback ke data dummy) telah diimplementasikan.
    - Penanda kapal di peta sekarang menampilkan warna berdasarkan tipe dan rotasi berdasarkan arah kapal.
- **Fitur F-002 (Pencarian Kapal)**: **Selesai**. Termasuk fungsionalitas *auto-complete*.
- **Fitur F-003 (Manajemen Layanan Pelabuhan)**: **Selesai**.
    - Pengajuan, persetujuan, dan penolakan permintaan layanan berfungsi.
    - Fungsionalitas unggah dokumen (menyalin ke folder aplikasi) telah diimplementasikan.
- **Otentikasi dan Otorisasi**: **Selesai**.
    - Alur Login, Logout, dan Registrasi berfungsi penuh.
    - Manajemen jendela terpusat menggunakan `IWindowManager`.
    - RBAC telah diperluas untuk memfilter data dan menyembunyikan elemen UI berdasarkan peran pengguna.
- **UI dan Tema**:
    - Sistem tema terang/gelap berfungsi dan pilihan pengguna disimpan antar sesi.
    - Semua tampilan utama (`Login`, `Register`, `MainWindow`, `ServiceRequest`, `MapView`) telah disempurnakan dengan gaya Fluent Design.
    - Animasi transisi *fade* telah ditambahkan.
- **Kualitas Kode**:
    - Proyek pengujian unit (`HarborFlow.Tests`) telah dibuat dan dikonfigurasi.
    - Unit test telah ditulis untuk `AuthService` dan semua ViewModel utama.
    - Semua peringatan saat build telah diperbaiki.
- **Layanan Aplikasi**:
    - `INotificationService` untuk notifikasi toast.
    - `ISettingsService` untuk menyimpan pengaturan pengguna.
    - `IFileService` untuk menangani penyimpanan dokumen.

### Langkah Selanjutnya:
- **Prioritas Utama**: Mengintegrasikan kunci API yang sebenarnya untuk data kapal real-time untuk memverifikasi fungsionalitas peta secara penuh.
- **Prioritas Kedua**: Menambahkan lebih banyak pengujian unit untuk cakupan yang lebih luas, terutama untuk logika yang baru ditambahkan.
- **Polesan Akhir**: Menambahkan detail kecil lainnya pada UI jika diperlukan.
