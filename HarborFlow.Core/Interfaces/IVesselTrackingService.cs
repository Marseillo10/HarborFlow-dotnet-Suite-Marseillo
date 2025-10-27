
using HarborFlow.Core.Models;
using System.Collections.ObjectModel;

namespace HarborFlow.Core.Interfaces
{
    public interface IVesselTrackingService
    {
        ObservableCollection<Vessel> TrackedVessels { get; }
        Task StartTracking(double[][] boundingBoxes);
        Task StopTracking();
        Task<Vessel?> GetVesselByImoAsync(string imo);
        Task<IEnumerable<Vessel>> SearchVesselsAsync(string searchTerm);
        Task<IEnumerable<Vessel>> GetAllVesselsAsync();
        Task<Vessel> AddVesselAsync(Vessel vessel);
        Task<Vessel> UpdateVesselAsync(Vessel vessel);
        Task DeleteVesselAsync(string imo);
        Task<IEnumerable<VesselPosition>> GetVesselHistoryAsync(string imo);

        event Action<VesselPosition> PositionReceived;
    }
}
