using HarborFlow.Wpf.ViewModels;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Views
{
    public partial class ServiceRequestView : UserControl
    {
        public ServiceRequestView(ServiceRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += async (s, e) => await viewModel.LoadServiceRequestsAsync();
        }
    }
}