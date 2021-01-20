using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;

namespace ChameraVote.ViewModels
{
    public class VotingSumResultsViewModel:BaseViewModel
    {
        public VotingResultsViewModel VotingResultsViewModel = null;

        public string Title
        {
            get { return this.VotingResultsViewModel.VotingViewModel.VotingTitle; }
        }

        private ObservableCollection<VotingOptionSum> votingOptionSums;

        public ObservableCollection<VotingOptionSum> VotingOptionSums
        {
            get { return this.votingOptionSums; }
            set { this.votingOptionSums = value; this.OnPropertyChanged(); }
        }

        private ObservableCollection<string> votingClients;

        public ObservableCollection<string> VotingClients
        {
            get { return this.votingClients; }
            set { this.votingClients = value; this.OnPropertyChanged(); }
        }

        public int CountOptionVotes(Collection<string> votes, string OptionValue)
        {
            int i = 0;
            foreach(var vote in votes)
            {
                if(vote==OptionValue)
                {
                    i++;
                }
            }
            return i;
        }

        public VotingSumResultsViewModel(VotingResultsViewModel votingmResultsViewModel)
        {
            this.VotingResultsViewModel = votingmResultsViewModel;
            this.votingOptionSums = new ObservableCollection<VotingOptionSum>();
            var options = this.VotingResultsViewModel.VotingViewModel.VotingOptionsRaw;
            foreach(var option in options)
            {
                VotingOptionSum votingOptionSum = new VotingOptionSum() { OptionValue = option, Votes = this.CountOptionVotes(this.VotingResultsViewModel.VotingViewModel.Results, option) };
                this.votingOptionSums.Add(votingOptionSum);
            }
            var clients = votingmResultsViewModel.VotingViewModel.Voters.Distinct<string>().ToList();
            if(votingmResultsViewModel.VotingViewModel.VotingModel.anonymous)
            {
                clients = votingmResultsViewModel.VotingViewModel.VoteSignedClients.Distinct<string>().ToList();
            }
            this.VotingClients = new ObservableCollection<string>(clients.ToList());
        }

        public class VotingOptionSum
        {
            private string optionValue="";

            public string OptionValue
            {
                get { return optionValue; }
                set { this.optionValue = value; }
            }

            public int votes = 0;
            public int Votes
            {
                get { return this.votes; }
                set { this.votes = value; }
            }
        }
    }
}
