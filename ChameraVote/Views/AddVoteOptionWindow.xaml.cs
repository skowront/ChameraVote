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
using ChameraVote.Models;

namespace ChameraVote.Views
{
    /// <summary>
    /// Interaction logic for AddVoteOptionWindow.xaml
    /// </summary>
    public partial class AddVoteOptionWindow : Window
    {
        public VoteOptionViewModel voteOptionViewModel = new VoteOptionViewModel(new VoteOption());

        public EventHandler OnItemAdd = null;

        public AddVoteOptionWindow()
        {
            InitializeComponent();
            this.DataContext = this.voteOptionViewModel;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            OnItemAdd?.Invoke(sender, e);
        }
    }
}
