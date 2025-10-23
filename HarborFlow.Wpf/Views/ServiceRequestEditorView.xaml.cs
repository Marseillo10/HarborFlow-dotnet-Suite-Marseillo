using HarborFlow.Wpf.ViewModels;
using System.Windows;

namespace HarborFlow.Wpf.Views
{
    public partial class ServiceRequestEditorView : Window
    {
        public ServiceRequestEditorView(ServiceRequestEditorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.CloseRequested += (result) => {
                DialogResult = result;
                Close();
            };
        }
    }
}
