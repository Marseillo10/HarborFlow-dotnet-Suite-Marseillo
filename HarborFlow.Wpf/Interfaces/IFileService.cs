
using System.Threading.Tasks;

namespace HarborFlow.Wpf.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveDocumentAsync(string sourceFilePath);
    }
}
