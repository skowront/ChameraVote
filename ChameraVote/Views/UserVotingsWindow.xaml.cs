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
using ChameraVote.Views;
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

        public bool GetUserVotings()
        {
            VoteClient voteClient = new VoteClient(this.ConfigurationViewModel);
            int ec = 0;
            var result = voteClient.GetUserVotingsBrief(this.UserViewModel.Username,this.UserViewModel.Token,string.Empty,out ec);
            if(result==null)
            {
                this.UserVotingsViewModel.BriefModels = new Collection<VotingBriefViewModel>();
                return false;
            }
            Collection<VotingBriefViewModel> collection = new Collection<VotingBriefViewModel>();
            foreach(var item in result)
            {
                var viewModel = new VotingBriefViewModel(item);
                collection.Add(viewModel);
            }
            this.UserVotingsViewModel.BriefModels = collection;
            return true;
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

        private void ResultsItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                VotingBriefViewModel viewModel = ((MenuItem)sender).DataContext as VotingBriefViewModel;
                VoteClient voteClient = new VoteClient(this.ConfigurationViewModel);
                int ec = 0;
                var result = voteClient.GetVotingModel(viewModel.Id,this.UserViewModel.Username, this.UserViewModel.Token, string.Empty, out ec);
                if ( result ==null )
                {
                    return;
                }
                VotingResultsWindow window = new VotingResultsWindow(new VotingViewModel(result),this.ConfigurationViewModel);
                window.ShowDialog();
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
           if(sender is MenuItem)
           {
                VotingBriefViewModel viewModel = ((MenuItem)sender).DataContext as VotingBriefViewModel;
                VoteClient voteClient = new VoteClient(this.ConfigurationViewModel);
                int ec = 0;
                voteClient.RemoveVoting(this.UserViewModel.Username, this.UserViewModel.Token, viewModel.Id, out ec);
                this.GetUserVotings();
           }
        }
    }
}
