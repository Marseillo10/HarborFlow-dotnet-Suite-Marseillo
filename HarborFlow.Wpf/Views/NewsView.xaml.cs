using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace HarborFlow.Wpf.Views
{
    public partial class NewsView : UserControl
    {
        public NewsView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
