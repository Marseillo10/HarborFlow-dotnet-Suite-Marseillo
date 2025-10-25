
using HarborFlow.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Views
{
    public partial class UserProfileView : UserControl
    {
        public UserProfileView()
        {
            InitializeComponent();
            Loaded += UserProfileView_Loaded;
        }

        private void UserProfileView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserProfileViewModel vm)
            {
                CurrentPasswordBox.PasswordChanged += (s, args) => vm.CurrentPassword = ((PasswordBox)s).Password;
                NewPasswordBox.PasswordChanged += (s, args) => vm.NewPassword = ((PasswordBox)s).Password;
                ConfirmPasswordBox.PasswordChanged += (s, args) => vm.ConfirmPassword = ((PasswordBox)s).Password;
            }
        }
    }
}
