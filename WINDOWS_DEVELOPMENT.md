# Tugas Pengembangan Khusus Windows untuk HarborFlow WPF

Dokumen ini menjelaskan tugas-tugas pengembangan yang spesifik untuk lingkungan Windows dan tidak dapat dilakukan oleh developer yang menggunakan macOS, karena sifat proyek ini yang berbasis Windows Presentation Foundation (WPF).

## Latar Belakang

Meskipun sebagian besar codebase (.NET Class Libraries) bersifat cross-platform dan dapat dibangun di macOS atau Linux, bagian antarmuka pengguna (UI) adalah aplikasi WPF. Ini menciptakan beberapa batasan bagi developer yang tidak menggunakan Windows.

## Tugas yang Memerlukan Windows

Berikut adalah daftar tugas penting yang harus dilakukan di lingkungan Windows untuk memastikan fungsionalitas dan kualitas aplikasi.

### 1. Menjalankan dan Melakukan Debugging Aplikasi

Aplikasi utama `HarborFlow.Wpf` hanya dapat dijalankan dan di-debug pada sistem operasi Windows. Ini adalah satu-satunya cara untuk memverifikasi secara visual bahwa UI berfungsi seperti yang diharapkan.

**Langkah-langkah:**
- Buka file solusi (`HarborFlow.sln`) di Visual Studio.
- Atur `HarborFlow.Wpf` sebagai "Startup Project".
- Jalankan aplikasi dengan menekan `F5` atau tombol "Start".

### 2. Menjalankan Unit Test

Meskipun proyek berhasil di-build di macOS, **eksekusi unit test gagal** karena memerlukan .NET Desktop Runtime (`Microsoft.WindowsDesktop.App`) yang tidak tersedia di macOS.

Menjalankan tes ini sangat penting untuk memvalidasi logika di ViewModels dan memastikan tidak ada regresi.

**Langkah-langkah:**
- Buka terminal (Command Prompt atau PowerShell) di direktori root proyek.
- Jalankan perintah berikut:
  ```shell
  dotnet test
  ```
- Atau, jalankan tes melalui **Test Explorer** di Visual Studio.

### 3. Verifikasi Visual dan Interaksi UI

Setiap perubahan pada file XAML (`.xaml`) yang mendefinisikan antarmuka pengguna—seperti tata letak, gaya, warna, dan data binding—harus diverifikasi secara visual dengan menjalankan aplikasi di Windows.

Ini adalah satu-satunya cara untuk memastikan bahwa:
- Tampilan sesuai dengan desain.
- Semua kontrol (tombol, daftar, dll.) dapat di-render dan berinteraksi dengan benar.
- Data binding antara View dan ViewModel berfungsi seperti yang diharapkan.

## Kesimpulan

Developer di macOS dapat berkontribusi pada logika bisnis di proyek `HarborFlow.Core`, `HarborFlow.Application`, dan `HarborFlow.Infrastructure`. Namun, setiap pekerjaan yang menyentuh UI (`HarborFlow.Wpf`) atau memerlukan verifikasi melalui tes (`HarborFlow.Tests`) harus divalidasi dan diuji di lingkungan Windows sebelum dianggap selesai.
