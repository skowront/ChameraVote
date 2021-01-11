using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ChameraVote.Models;

namespace ChameraVote.ViewModels
{
    public class UserVotingsViewModel:BaseViewModel
    {
        private Collection<VotingBriefViewModel> briefModels = new Collection<VotingBriefViewModel>();

        public Collection<VotingBriefViewModel> BriefModels
        {
            get { return this.briefModels; }
            set { this.briefModels = value; this.OnPropertyChanged(); }
        }
    }
}
