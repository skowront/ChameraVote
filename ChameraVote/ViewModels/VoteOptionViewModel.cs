using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;

namespace ChameraVote.ViewModels
{
    public class VoteOptionViewModel:BaseViewModel
    {
        private VoteOption voteOption = new VoteOption();

        public string OptionValue
        {
            get { return this.voteOption.optionValue; }
            set { this.voteOption.optionValue = value; this.OnPropertyChanged(); }
        }

        public bool OptionChecked
        {
            get { return this.voteOption.optionChecked; }
            set { this.voteOption.optionChecked = value; this.OnPropertyChanged(); }
        }

        public EventHandler CheckedEventHandler = null;

        public VoteOptionViewModel(VoteOption voteOption)
        {
            this.voteOption = voteOption;
        }

        protected override void OnPropertyChanged([CallerMemberName] string property = null)
        {
            base.OnPropertyChanged(property);
            if(property == nameof(this.OptionChecked))
            {
                if(this.OptionChecked==true)
                {
                    this.CheckedEventHandler?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
