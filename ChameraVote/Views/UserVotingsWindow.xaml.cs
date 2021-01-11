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
using System.Collections.ObjectModel;
using ChameraVote.ViewModels;
using ChameraVote.Utility;

namespace ChameraVote.Views
{
    /// <summary>
    /// Interaction logic for UserVotingsWindow.xaml
    /// </summary>
    public partial class UserVotingsWindow : Window
    {
        public UserVotingsViewModel UserVotingsViewModel = new UserVotingsViewModel();

        public UserViewModel UserViewModel = new UserViewModel();

        public ConfigurationViewModel ConfigurationViewModel = new ConfigurationViewModel();

        public UserVotingsWindow(UserViewModel userViewModel,ConfigurationViewModel configurationViewModel)
        {
            this.UserViewModel = userViewModel;
            this.ConfigurationViewModel = configurationViewModel;
            this.GetUserVotings();
            InitializeComponent();
            this.DataContext = this.UserVotingsViewModel;
        }

        public void GetUserVotings()
        {
            VoteClient voteClient = new VoteClient(this.ConfigurationViewModel.ServerAddress);
            var result = voteClient.GetUserVotingsBrief(this.UserViewModel.Username,this.UserViewModel.Token,string.Empty);
            if(result==null)
            {
                this.UserVotingsViewModel.BriefModels = new Collection<VotingBriefViewModel>();
                return;
            }
            Collection<VotingBriefViewModel> collection = new Collection<VotingBriefViewModel>();
            foreach(var item in result)
            {
                var viewModel = new VotingBriefViewModel(item);
                collection.Add(viewModel);
            }
            this.UserVotingsViewModel.BriefModels = collection;
        }

        private void votingsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void newVotingButton_Click(object sender, RoutedEventArgs e)
        {
            NewVotingWindow window = new NewVotingWindow(this.ConfigurationViewModel,this.UserViewModel);
            window.ShowDialog();
            this.GetUserVotings();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
           if(sender is MenuItem)
           {
                VotingBriefViewModel viewModel = ((MenuItem)sender).DataContext as VotingBriefViewModel;
                VoteClient voteClient = new VoteClient(this.ConfigurationViewModel.ServerAddress);
                voteClient.RemoveVoting(this.UserViewModel.Username, this.UserViewModel.Token, viewModel.Id);
                this.GetUserVotings();
           }
        }
    }
}
