using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace ChameraVote
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var theme = Theme.Create(Theme.Dark, Color.FromRgb(0, 76, 151),Color.FromRgb(255,205,0));
            var paletteHelper = new PaletteHelper();
            paletteHelper.SetTheme(theme);
        }
    }
}
