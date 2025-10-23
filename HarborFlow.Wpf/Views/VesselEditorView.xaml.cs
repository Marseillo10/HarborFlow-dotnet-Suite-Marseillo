using HarborFlow.Wpf.ViewModels;
using System.Windows;

namespace HarborFlow.Wpf.Views
{
    public partial class VesselEditorView : Window
    {
        public VesselEditorView(VesselEditorViewModel viewModel)
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
