using System.Threading.Tasks;
using HarborFlow.Core.Models;

namespace HarborFlow.Core.Interfaces
{
    public interface IAisDataService
    {
        Task<Vessel> GetVesselDataAsync(string imo);
    }
}