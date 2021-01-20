using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;
using ChameraVote.Utility;

namespace ChameraVote.ViewModels
{
    public class VotingViewModel:BaseViewModel,IPropertyStringValid
    {
        private VotingModel votingModel = new VotingModel();

        public VotingModel VotingModel
        {
            get { this.votingModel.votingOptionsRaw = new Collection<string>(this.votingOptionsRaw); return this.votingModel; }
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
            set { this.votingModel.owner = value; this.OnPropertyChanged(); }
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
            set { this.votingModel.anonymous = value; this.OnPropertyChanged(); }
        }

        public bool MutuallyExclusive
        {
            get { return this.votingModel.mutuallyExclusive; }
            set { this.votingModel.mutuallyExclusive = value; this.OnPropertyChanged(); }
        }
        public bool AllowUnregisteredUsers
        {
            get { return this.votingModel.allowUnregisteredUsers; }
            set { this.votingModel.allowUnregisteredUsers = value; this.OnPropertyChanged(); }
        }

        public int MaxOptions
        {
            get { return this.votingModel.maxOptions; }
            set { this.votingModel.maxOptions = value; this.OnPropertyChanged(); }
        }

        public string Password
        {
            get { return this.votingModel.password; }
            set { this.votingModel.password = value;this.OnPropertyChanged(); }
        }

        public string BallotId
        {
            get { return this.votingModel.ballotId; }
            set { this.votingModel.ballotId = value; this.OnPropertyChanged(); }
        }

        public int BlindFactor
        {
            get { return this.votingModel.blindFactor; }
            set { this.votingModel.blindFactor = value; this.OnPropertyChanged(); }
        }

        public string Signature
        {
            get { return this.votingModel.signature; }
            set { this.votingModel.signature = value; this.OnPropertyChanged(); }
        }

        public Collection<string> Voters
        {
            get { return this.votingModel.votingClients; }
            set { this.votingModel.votingClients = value; this.OnPropertyChanged(); }
        }

        public Collection<string> Results
        {
            get { return this.votingModel.votingResults; }
            set { this.votingModel.votingResults = value; this.OnPropertyChanged(); }
        }

        private string status = "";

        public string Status
        {
            get { return this.status; }
            set { this.status = value; this.OnPropertyChanged(); }
        }

        private ObservableCollection<string> votingOptionsRaw = null;

        public ObservableCollection<string> VotingOptionsRaw
        {
            get 
            {
                if (this.votingOptionsRaw==null)
                {
                    this.votingOptionsRaw = new ObservableCollection<string>(this.votingModel.votingOptionsRaw);
                }
                return this.votingOptionsRaw; 
            }
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
                this.votingOptionsRaw = new ObservableCollection<string>(this.votingModel.votingOptionsRaw);
                this.OnPropertyChanged(); 
            }
        }

        private Collection<VoteOptionViewModel> voteOptionViewModels;

        public Collection<VoteOptionViewModel> VoteOptionViewModels
        {
            get { return this.voteOptionViewModels; }
            set { this.voteOptionViewModels = value; this.OnPropertyChanged(); }
        }

        public Collection<string> VoteSignedClients
        {
            get { return this.votingModel.votingSignedClients; }
            set { this.votingModel.votingSignedClients = value; this.OnPropertyChanged(); }
        }

        public VotingViewModel()
        {
        }

        public VotingViewModel(VotingModel votingModel)
        {
            this.VotingModel = votingModel;
            Random rnd = new Random();
            this.votingModel.blindFactor = rnd.Next(0,100); 
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
        public bool PropertiesValid()
        {
            bool val = true;
            if (this.VotingId.Contains(':') || this.Password.Contains(':') || this.VotingTitle.Contains(':'))
            {
                this.Status = "':' not allowed.";
                val = false;
            }
            return val;
        }
    }
}
