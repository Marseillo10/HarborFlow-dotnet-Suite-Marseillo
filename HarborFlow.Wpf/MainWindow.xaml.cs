using System.Windows;
using System.Windows.Controls;
using HarborFlow.Wpf.Views;
using HarborFlow.Wpf.ViewModels;
using System;

using HarborFlow.Wpf.Interfaces;

using System.Windows.Shapes;
using System.Windows.Media;

using System.Threading.Tasks;

namespace HarborFlow.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
