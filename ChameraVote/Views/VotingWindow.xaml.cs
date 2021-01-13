using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ChameraVote.Utility;
using ChameraVote.ViewModels;

namespace ChameraVote.Views
{
    /// <summary>
    /// Interaction logic for VotingWindow.xaml
    /// </summary>
    public partial class VotingWindow : Window
    {
        public UserViewModel userViewModel = new UserViewModel();

        public VotingViewModel VotingViewModel = new VotingViewModel();

        public ConfigurationViewModel ConfigurationViewModel = new ConfigurationViewModel();

        public UserViewModel UserViewModel
        {
            get { return this.userViewModel; }
            set { this.userViewModel = value; this.usernameTextBox.DataContext = this.UserViewModel; }
        }

        public VotingWindow()
        {
            InitializeComponent();
        }

        public VotingWindow(UserViewModel userViewModel,ConfigurationViewModel configuration)
        {
            
            InitializeComponent();
            this.DataContext = this.VotingViewModel;
            this.usernameTextBox.DataContext = this.UserViewModel;
            this.UserViewModel = userViewModel;
            this.ConfigurationViewModel = configuration;
            if(this.userViewModel.Token!=string.Empty)
            {
                this.usernameTextBox.IsEnabled = false;
            }
            this.serverAddressTextBox.DataContext = this.ConfigurationViewModel;
            this.userViewModel.PropertyChanged += (s, e) => { this.usernameTextBox.DataContext = this.UserViewModel; };
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.VotingViewModel.PropertiesValid())
            {
                return;
            }
            VoteClient voteClient = new VoteClient(this.ConfigurationViewModel);
            int ec = 0;
            var model = voteClient.GetVotingModel(this.VotingViewModel.VotingId, this.UserViewModel.Username, this.UserViewModel.Token, this.VotingViewModel.Password, out ec);
            if (model == null)
            {
                this.votingMainStackPanel.Visibility = Visibility.Hidden;
                return;
            }
            this.VotingViewModel = new VotingViewModel();
            this.VotingViewModel.VotingModel = model;
            this.DataContext = this.VotingViewModel;
            this.votingMainStackPanel.Visibility = Visibility.Visible;
        }

        private void applyServerAddress_Click(object sender, RoutedEventArgs e)
        {
            this.VotingViewModel.ServerAddress = this.serverAddressTextBox.Text;
        }

        private void sendVote_Click(object sender, RoutedEventArgs e)
        {
            VoteClient voteClient = new VoteClient(this.ConfigurationViewModel);
            Collection<string> voteOptionsSelected = new Collection<string>();
            if(this.VotingViewModel.VoteOptionViewModels==null)
            {
                return;
            }
            foreach (var item in this.VotingViewModel.VoteOptionViewModels)
            {
                if (item.OptionChecked == true)
                {
                    voteOptionsSelected.Add(item.OptionValue);
                }
            }
            int ec = 0;
            var result = voteClient.SendVote(this.VotingViewModel.VotingId, voteOptionsSelected, this.UserViewModel.Username, this.UserViewModel.Token, this.VotingViewModel.Password, out ec);
            if(result == true)
            {
                this.VotingViewModel.Status = "Vote accepted.";
            }
            else
            {
                this.VotingViewModel.Status = VoteClient.errors[ec].Item2;
            }
        }
    }
}
