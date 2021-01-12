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

namespace ChameraVote.Views
{
    /// <summary>
    /// Interaction logic for VotingSumResultsWindow.xaml
    /// </summary>
    public partial class VotingSumResultsWindow : Window
    {
        public VotingSumResultsViewModel VotingSumResultsViewModel = null;

        public VotingSumResultsWindow(VotingResultsViewModel votingResultsViewModel)
        {
            this.VotingSumResultsViewModel = new VotingSumResultsViewModel(votingResultsViewModel);
            InitializeComponent();
            this.DataContext = this.VotingSumResultsViewModel;
        }
    }
}
