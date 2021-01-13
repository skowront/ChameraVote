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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginViewModel LoginViewModel = new LoginViewModel();

        public ConfigurationViewModel ConfigurationViewModel = new ConfigurationViewModel();

        public EventHandler OnLoginSuccess = null;

        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = LoginViewModel;
        }

        public LoginWindow(ConfigurationViewModel configurationViewModel)
        {
            InitializeComponent();
            this.ConfigurationViewModel = configurationViewModel;
            this.DataContext = LoginViewModel;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!this.LoginViewModel.PropertiesValid())
            {
                return;
            }
            VoteClient voteClient = new VoteClient(this.ConfigurationViewModel);
            if(this.userPasswordTextBox.Password.Contains(':'))
            {
                this.statusTextBox.Text = "':' not allowed in passsowrd";
                return;
            }
            int ec = 0;
            var token = voteClient.Login(this.LoginViewModel.Username, this.userPasswordTextBox.Password, out ec);
            if(token == null)
            {
                this.LoginViewModel.Status = "Login failed.";
                return;
            }
            else
            {
                this.LoginViewModel.Status = "Login successful.";
                this.LoginViewModel.Token = token;  
                this.OnLoginSuccess?.Invoke(sender,e);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Topmost = false;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.Topmost = true;
        }
    }
}
