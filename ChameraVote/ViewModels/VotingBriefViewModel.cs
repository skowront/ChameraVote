using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;

namespace ChameraVote.ViewModels
{
    public class VotingBriefViewModel:BaseViewModel
    {
        private VotingBriefModel votingBriefModel = new VotingBriefModel();

        public string Title
        {
            get { return this.votingBriefModel.title; }
            set { this.votingBriefModel.title = value; this.OnPropertyChanged(); }
        }

        public string Id
        {
            get { return this.votingBriefModel.id; }
            set { this.votingBriefModel.id = value; this.OnPropertyChanged(); }
        }

        public VotingBriefViewModel()
        {

        }

        public VotingBriefViewModel(VotingBriefModel votingBriefModel)
        {
            this.votingBriefModel = votingBriefModel;
        }
    }
}
