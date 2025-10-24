
using HarborFlow.Core.Models;

namespace HarborFlow.Wpf.Interfaces
{
    public interface IWindowManager
    {
        void ShowLoginWindow();
        void ShowMainWindow();
        void ShowRegisterWindow();
        void CloseLoginWindow();
        bool? ShowVesselEditorDialog(Vessel vessel);
        bool? ShowServiceRequestEditorDialog(ServiceRequest serviceRequest);
        string? ShowInputDialog(string title, string message);
    }
}
