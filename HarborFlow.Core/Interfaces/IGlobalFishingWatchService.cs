using HarborFlow.Core.Models;
using System.Threading.Tasks;

namespace HarborFlow.Core.Interfaces
{
    public interface IGlobalFishingWatchService
    {
        Task<VesselType> GetVesselTypeAsync(string imo);
        Task<Dictionary<string, VesselType>> GetVesselTypesAsync(List<string> imos);
    }
}
