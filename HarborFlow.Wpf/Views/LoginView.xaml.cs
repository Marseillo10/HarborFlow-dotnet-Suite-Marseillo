using System.Windows;
using System.Windows.Controls;
using HarborFlow.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace HarborFlow.Wpf.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.AppHost != null)
            {
                var registerView = App.AppHost.Services.GetRequiredService<RegisterView>();
                registerView.Show();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel && sender is PasswordBox passwordBox)
            {
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}
