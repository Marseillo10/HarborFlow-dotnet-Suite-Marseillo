
using HarborFlow.Core.Models;

namespace HarborFlow.Wpf.Interfaces
{
    public interface IWindowManager
    {
        void ShowLoginWindow();
        void ShowMainWindow();
        bool? ShowVesselEditorDialog(Vessel vessel);
        bool? ShowServiceRequestEditorDialog(ServiceRequest serviceRequest);
    }
}
