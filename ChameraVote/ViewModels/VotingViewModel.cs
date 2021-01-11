using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;

namespace ChameraVote.ViewModels
{
    public class VotingViewModel:BaseViewModel
    {
        private VotingModel votingModel = new VotingModel();

        public VotingModel VotingModel
        {
            get { return this.votingModel; }
            set { this.votingModel = value; this.OnPropertyChanged(); }
        }

        public string ServerAddress
        {
            get { return this.votingModel.serverAddress; }
            set { this.votingModel.serverAddress = value; this.OnPropertyChanged(); }
        }

        public string Owner
        {
            get { return this.votingModel.owner; }
        }

        public string VotingTitle
        {
            get { return this.votingModel.votingTitle; }
            set { this.votingModel.votingTitle = value; this.OnPropertyChanged(); }
        }

        public string VotingId
        {
            get { return this.votingModel.votingId; }
            set { this.votingModel.votingId = value;this.OnPropertyChanged(); }
        }

        public bool Anonymous
        {
            get { return this.votingModel.anonymous; }
        }

        public bool MutuallyExclusive
        {
            get { return this.votingModel.mutuallyExclusive; }
        }
        public bool AllowUnregisteredUsers
        {
            get { return this.votingModel.allowUnregisteredUsers; }
        }

        public string Password
        {
            get { return this.votingModel.password; }
            set { this.votingModel.password = value;this.OnPropertyChanged(); }
        }

        public Collection<string> VotingOptionsRaw
        {
            get { return this.votingModel.votingOptionsRaw; }
            set 
            { 
                this.votingModel.votingOptionsRaw = value;
                Collection<VoteOptionViewModel> newOptions = new Collection<VoteOptionViewModel>();
                foreach (var item in votingModel.votingOptionsRaw)
                {
                    var newOption = new VoteOptionViewModel(new VoteOption(item));
                    if(this.MutuallyExclusive==true)
                    {
                        newOption.CheckedEventHandler += this.MutuallyExclusiveOnCheckedEventHandler;
                    }
                    newOptions.Add(newOption);
                }
                this.VoteOptionViewModels = newOptions;
                this.OnPropertyChanged(); 
            }
        }

        private Collection<VoteOptionViewModel> voteOptionViewModels;

        public Collection<VoteOptionViewModel> VoteOptionViewModels
        {
            get { return this.voteOptionViewModels; }
            set { this.voteOptionViewModels = value; this.OnPropertyChanged(); }
        }

        public VotingViewModel()
        {

        }

        protected override void OnPropertyChanged([CallerMemberName] string property = null)
        {
            base.OnPropertyChanged(property);
            if(property==nameof(this.VotingModel))
            {
                this.VotingOptionsRaw = this.VotingOptionsRaw;
            }
        }

        void MutuallyExclusiveOnCheckedEventHandler (object sender, EventArgs e)
        {
            if(!(sender is VoteOptionViewModel))
            {
                return;
            }
            foreach( var item in this.voteOptionViewModels)
            {
                if(item.OptionValue == ((VoteOptionViewModel)sender).OptionValue)
                {
                    continue;
                }
                item.OptionChecked = false;
            }
        }
    }
}
