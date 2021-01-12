using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChameraVote.ViewModels;
using ChameraVote.Utility;

namespace ChameraVote.Views
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public UserRegistrationViewModel userRegistrationViewModel = new UserRegistrationViewModel();

        public ConfigurationViewModel configurationViewModel = new ConfigurationViewModel();

        public RegistrationWindow(ConfigurationViewModel configurationViewModel)
        {
            this.configurationViewModel = configurationViewModel;
            InitializeComponent();
            this.DataContext = this.userRegistrationViewModel;
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            if(!this.userRegistrationViewModel.PropertiesValid())
            {
                return;
            }
            if (this.passwordBox.Password.Contains(':'))
            {
                this.userRegistrationViewModel.Status = "':' not allowed";
                return;
            }
            if(this.userRegistrationViewModel.Username==string.Empty)
            {
                this.userRegistrationViewModel.Status = "Enter username.";
                return;
            }
            if(this.userRegistrationViewModel.CheckPasswordStrength(this.passwordBox.Password)==UserRegistrationViewModel.PasswordStrength.bad)
            {
                this.userRegistrationViewModel.Status = "Password is weak.";
                return;
            }
            if(this.userRegistrationViewModel.Username.Contains(':') || this.userRegistrationViewModel.Status.Contains(':') || this.userRegistrationViewModel.RegistrationToken.Contains(':'))
            {
                this.userRegistrationViewModel.Status = "Sign ':' is not allowed.";
                return;
            }
            
            VoteClient voteClient = new VoteClient(configurationViewModel);
            var result = voteClient.Register(this.userRegistrationViewModel.Username, this.passwordBox.Password, this.userRegistrationViewModel.RegistrationToken);
            if(result==null)
            {
                return;
            }
        }
    }
}
