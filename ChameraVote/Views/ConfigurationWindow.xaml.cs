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
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationViewModel configurationViewModel = new ConfigurationViewModel();

        public EventHandler OnConfigurationChanged = null;

        public ConfigurationWindow()
        {
            InitializeComponent();
            this.DataContext = this.configurationViewModel;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        private void saveConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnConfigurationChanged?.Invoke(sender,e);
        }
    }
}
