
using HarborFlow.Wpf.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HarborFlow.Wpf.Services
{
    public class FileService : IFileService
    {
        private readonly string _documentsStoragePath;

        public FileService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolderPath = Path.Combine(appDataPath, "HarborFlow");
            _documentsStoragePath = Path.Combine(appFolderPath, "Documents");
            Directory.CreateDirectory(_documentsStoragePath);
        }

        public async Task<string> SaveDocumentAsync(string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("Source file not found.", sourceFilePath);
            }

            var fileName = Path.GetFileName(sourceFilePath);
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var destinationPath = Path.Combine(_documentsStoragePath, uniqueFileName);

            await Task.Run(() => File.Copy(sourceFilePath, destinationPath));

            // Return the unique file name to be stored in the database
            return uniqueFileName;
        }
    }
}
