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
using ChameraVote.ViewModels;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ChameraVote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserViewModel userViewModel = new UserViewModel();

        private ConfigurationViewModel configurationViewModel = new ConfigurationViewModel();

        public UserViewModel UserViewModel
        {
            get { return this.userViewModel; }
            set { this.userViewModel = value; this.OnPropertyChanged(new DependencyPropertyChangedEventArgs()); }
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        public void PopupLogin()
        {
            LoginWindow window = new LoginWindow();
            window.OnLoginSuccess += (o, e) =>
            {
                this.UserViewModel.Username = window.LoginViewModel.Username;
                this.UserViewModel.Token = window.LoginViewModel.Token;
                this.IsEnabled = true;
            };
            window.ShowDialog();
        }
        public void PopupVote()
        {
            VotingWindow window = new VotingWindow();
            window.votingPage.UserViewModel = this.UserViewModel;
            
            window.ShowDialog();
        }

        public void PopupConfiguration()
        {
            ConfigurationWindow window = new ConfigurationWindow();
            window.OnConfigurationChanged += (o, e) =>
            {
                this.configurationViewModel = window.configurationViewModel;
            };
            window.ShowDialog();
        }

        public void PopupUserVotings()
        {
            UserVotingsWindow window = new UserVotingsWindow();
            window.ShowDialog();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            this.PopupLogin();
        }

        private void voteButton_Click(object sender, RoutedEventArgs e)
        {
            this.PopupVote();
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.PopupConfiguration();
        }

        private void myVotingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.PopupUserVotings();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
