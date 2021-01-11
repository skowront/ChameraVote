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

        public EventHandler OnLoginSuccess = null;

        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = LoginViewModel;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            VoteClient voteClient = new VoteClient(this.LoginViewModel.ServerAddress);
            var token = voteClient.Login(this.LoginViewModel.Username, this.userPasswordTextBox.Password);
            if(token == null)
            {
                return;
            }
            else
            {
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
