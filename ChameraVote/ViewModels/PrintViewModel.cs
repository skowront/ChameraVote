using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.ViewModels;
using System.Collections.ObjectModel;

namespace ChameraVote.ViewModels
{
    public class PrintViewModel : BaseViewModel
    {
        private VotingSumResultsViewModel votingSumResultsViewModel;

        private ObservableCollection<string> languages = new ObservableCollection<string>(new List<string>{"PL", "EN"});
        
        public ObservableCollection<string> Languages
        {
            get { return this.languages; }
            set { this.languages = value; this.OnPropertyChanged(); }
        }

        public VotingSumResultsViewModel VotingSumResultsViewModel
        {
            get { return this.votingSumResultsViewModel; }
            set { this.votingSumResultsViewModel = value; this.OnPropertyChanged(); }
        }
        
        public PrintViewModel(VotingSumResultsViewModel votingSumResultsViewModel)
        {
            this.votingSumResultsViewModel = votingSumResultsViewModel;
        }
    }
}
