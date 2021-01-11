
using System.Windows;
using System.Windows.Controls;
using ChameraVote.ViewModels;
using ChameraVote.Utility;
using System.Collections.ObjectModel;

namespace ChameraVote.Views
{
    /// <summary>
    /// Interaction logic for VotingPage.xaml
    /// </summary>
    public partial class VotingPage : Page
    {
        public VotingViewModel VotingViewModel = new VotingViewModel();

        public UserViewModel userViewModel = new UserViewModel();

        public ConfigurationViewModel ConfigurationViewModel = new ConfigurationViewModel();

        public UserViewModel UserViewModel
        {
            get { return this.userViewModel; }
            set { this.userViewModel = value; this.usernameTextBox.DataContext = this.UserViewModel; }
        }

        public VotingPage()
        {
            InitializeComponent();
            this.DataContext = this.VotingViewModel;
            this.usernameTextBox.DataContext = this.UserViewModel;
            this.serverAddressTextBox.DataContext = this.ConfigurationViewModel;
            this.userViewModel.PropertyChanged += (s, e) => { this.usernameTextBox.DataContext = this.UserViewModel; };
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            VoteClient voteClient = new VoteClient(this.ConfigurationViewModel.ServerAddress);
            var model = voteClient.GetVotingModel(this.VotingViewModel.VotingId,this.UserViewModel.Username,this.UserViewModel.Token,this.VotingViewModel.Password);
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
            VoteClient voteClient = new VoteClient(this.ConfigurationViewModel.ServerAddress);
            Collection<string> voteOptionsSelected = new Collection<string>();
            foreach (var item in this.VotingViewModel.VoteOptionViewModels)
            {
                if(item.OptionChecked==true)
                {
                    voteOptionsSelected.Add(item.OptionValue);
                }
            }
            voteClient.SendVote(this.VotingViewModel.VotingId,voteOptionsSelected,this.UserViewModel.Username,this.UserViewModel.Token,this.VotingViewModel.Password);
        }
    }
}
