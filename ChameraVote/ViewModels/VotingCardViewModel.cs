using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;

namespace ChameraVote.ViewModels
{
    public class VotingCardViewModel:BaseViewModel
    {
        public VotingCardModel VotingCardModel = new VotingCardModel();

        public string Username
        {
            get { return this.VotingCardModel.username; }
            set { this.VotingCardModel.username = value; this.OnPropertyChanged(); }
        }

        private ObservableCollection<string> options = new ObservableCollection<string>();

        public ObservableCollection<string> Options
        {
            get { return this.options; }
            set { this.options = value; this.OnPropertyChanged(); }
        }

        public VotingCardViewModel(Collection<string> voters, Collection<string> options, string username)
        {
            VotingCardModel votingCardModel = new VotingCardModel();
            for (int i = 0; i<voters.Count;i++)
            {
                if(voters[i]==username)
                {
                    votingCardModel.username = username;
                    votingCardModel.options.Add(options[i]);
                }
            }
            this.VotingCardModel = votingCardModel;
            this.options = new ObservableCollection<string>(votingCardModel.options);
        }
    }
}
