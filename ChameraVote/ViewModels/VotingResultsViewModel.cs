using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ChameraVote.ViewModels
{
    public class VotingResultsViewModel:BaseViewModel
    {
        public VotingViewModel VotingViewModel = new VotingViewModel();

        private ObservableCollection<VotingCardViewModel> votes = new ObservableCollection<VotingCardViewModel>();

        public ObservableCollection<VotingCardViewModel> Votes
        {
            get { return this.votes; }
            set { this.votes = value; this.OnPropertyChanged(); }
        }

        public VotingResultsViewModel(VotingViewModel votingViewModel)
        {
            this.VotingViewModel = votingViewModel;
            var uniqueVoters = votingViewModel.Voters.Distinct();
            foreach (var uniqueVoter in uniqueVoters)
            {
                VotingCardViewModel votingCard = new VotingCardViewModel(this.VotingViewModel.Voters,this.VotingViewModel.Results,uniqueVoter);
                this.Votes.Add(votingCard);
            }
        }
    }
}
