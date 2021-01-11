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
    /// Interaction logic for NewVotingWindow.xaml
    /// </summary>
    public partial class NewVotingWindow : Window
    {
        public VotingViewModel votingViewModel = new VotingViewModel();

        public UserViewModel userViewModel = new UserViewModel();

        public ConfigurationViewModel configurationViewModel = new ConfigurationViewModel();

        public NewVotingWindow(ConfigurationViewModel configurationViewModel, UserViewModel userViewModel)
        {
            this.userViewModel = userViewModel;
            this.configurationViewModel = configurationViewModel;
            this.votingViewModel.Owner = this.userViewModel.Username;
            InitializeComponent();
            this.DataContext = this.votingViewModel;
        }

        private void addItem_Click(object sender, RoutedEventArgs e)
        {
            AddVoteOptionWindow window = new AddVoteOptionWindow();
            window.OnItemAdd += (s, a) =>
            {
                this.votingViewModel.VotingOptionsRaw.Add(window.voteOptionViewModel.OptionValue);
                window.Close();
            };
            window.ShowDialog();
        }

        private void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            VoteClient voteClient = new VoteClient(this.configurationViewModel.ServerAddress);
            voteClient.AddNewVoting(userViewModel.Username,userViewModel.Token,votingViewModel.VotingModel);
        }
    }
}
