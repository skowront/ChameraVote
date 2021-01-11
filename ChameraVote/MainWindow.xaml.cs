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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChameraVote.Views;

namespace ChameraVote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.votingPage.IsEnabled = false;
            PopupLogin();
        }

        public void PopupLogin()
        {
            LoginWindow window = new LoginWindow();
            window.OnLoginSuccess += (o, e) =>
            {
                this.votingPage.VotingViewModel.Username = window.LoginViewModel.Username;
                this.votingPage.VotingViewModel.Token = window.LoginViewModel.Token;
                this.votingPage.IsEnabled = true; window.Close();
            };
            window.Show();
            window.BringIntoView();
            window.Topmost = true;
            window.StateChanged += Window_StateChanged;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(sender is LoginWindow)
            {
                ((LoginWindow)sender).Topmost = false;
            }
        }
    }
}
