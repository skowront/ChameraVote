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
    /// Interaction logic for VerifyWindow.xaml
    /// </summary>
    public partial class VerifyWindow : Window
    {
        private VotingResultsViewModel votingResultsViewModel = null;

        private ConfigurationViewModel configurationViewModel = null;

        public VerifyWindow()
        {
            InitializeComponent();
        }

        public VerifyWindow(VotingResultsViewModel votingResultsViewModel, ConfigurationViewModel configurationViewModel)
        {
            this.votingResultsViewModel = votingResultsViewModel;
            this.configurationViewModel = configurationViewModel;
            InitializeComponent();
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            VoteClient voteClient = new VoteClient(this.configurationViewModel);
            int errorCode = 0;
            var result = voteClient.Verify(this.votingResultsViewModel.VotingViewModel.VotingId, this.voteId.Text, this.signature.Text, out errorCode);
        }
    }
}
