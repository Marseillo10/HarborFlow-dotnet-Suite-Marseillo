using HarborFlow.Wpf.ViewModels;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView(DashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += async (s, e) => await viewModel.LoadDataAsync();
        }
    }
}
