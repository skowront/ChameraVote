﻿using System;
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
    /// Interaction logic for VotingResultsWindow.xaml
    /// </summary>
    public partial class VotingResultsWindow : Window
    {
        public VotingResultsViewModel VotingResultsViewModel = null;

        public ConfigurationViewModel ConfigurationViewModel = null;

        public VotingResultsWindow(VotingViewModel votingViewModel, ConfigurationViewModel configuration)
        {
            this.ConfigurationViewModel = configuration;
            this.VotingResultsViewModel = new VotingResultsViewModel(votingViewModel);
            InitializeComponent();
            this.DataContext = VotingResultsViewModel;
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            PrintWindow window = new PrintWindow(new VotingSumResultsViewModel(this.VotingResultsViewModel));
            window.ShowDialog();
        }

        private void sumResults_Click(object sender, RoutedEventArgs e)
        {
            VotingSumResultsWindow window = new VotingSumResultsWindow(this.VotingResultsViewModel);
            window.ShowDialog();
        }

        private void verifySignature_Click(object sender, RoutedEventArgs e)
        {
            VerifyWindow window = new VerifyWindow(this.VotingResultsViewModel,this.ConfigurationViewModel);
            window.ShowDialog();
        }
    }
}
