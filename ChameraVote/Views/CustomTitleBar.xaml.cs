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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChameraVote.Views
{
    /// <summary>
    /// Interaction logic for CustomTitleBar.xaml
    /// </summary>
    public partial class CustomTitleBar : UserControl
    {
        public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(nameof(Window), typeof(Window), typeof(CustomTitleBar));

        public Window Window
        {
            get => (Window)this.GetValue(WindowProperty);
            set => this.SetValue(WindowProperty, value);
        }

        public EventHandler MouseDownEventHandler = null;

        public EventHandler MouseDoubleClickEventHandler = null;

        public EventHandler MinimizeClickEventHandler = null;

        public EventHandler CloseClickEventHandler = null;

        public CustomTitleBar()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                this.MouseDoubleClickEventHandler?.Invoke(sender, e);
                this.Window.WindowState = this.Window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
            else if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.MouseDownEventHandler?.Invoke(sender, e);
                this.Window.DragMove();
            }
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.MinimizeClickEventHandler?.Invoke(sender, e);
            this.Window.WindowState = WindowState.Minimized;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.CloseClickEventHandler?.Invoke(sender, e);
            this.Window.Close();
        }
    }
}
