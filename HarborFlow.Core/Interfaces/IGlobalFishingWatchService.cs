using HarborFlow.Core.Models;
using System.Threading.Tasks;

namespace HarborFlow.Core.Interfaces
{
    public interface IGlobalFishingWatchService
    {
        Task<VesselType> GetVesselTypeAsync(string imo);
    }
}
