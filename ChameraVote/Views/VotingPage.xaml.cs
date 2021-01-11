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
using System.Net.Sockets;
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

        public VotingPage()
        {
            InitializeComponent();
            this.DataContext = this.VotingViewModel;
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            VoteClient voteClient = new VoteClient(this.VotingViewModel.ServerAddress);
            var model = voteClient.GetVotingModel(this.VotingViewModel.VotingId);
            if (model == null)
            {
                return;
            }
            this.VotingViewModel = new VotingViewModel();
            this.VotingViewModel.VotingModel = model;
            this.DataContext = this.VotingViewModel;
        }

        private void applyServerAddress_Click(object sender, RoutedEventArgs e)
        {
            this.VotingViewModel.ServerAddress = this.serverAddressTextBox.Text;
        }

        private void sendVote_Click(object sender, RoutedEventArgs e)
        {
            VoteClient voteClient = new VoteClient(this.VotingViewModel.ServerAddress);
            Collection<string> voteOptionsSelected = new Collection<string>();
            foreach (var item in this.VotingViewModel.VoteOptionViewModels)
            {
                if(item.OptionChecked==true)
                {
                    voteOptionsSelected.Add(item.OptionValue);
                }
            }
            voteClient.SendVote(this.VotingViewModel.VotingId,voteOptionsSelected,this.VotingViewModel.Username);
        }
    }
}
