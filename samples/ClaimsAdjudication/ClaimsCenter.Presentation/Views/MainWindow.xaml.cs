using System.Windows;
using NRules.Samples.ClaimsCenter.Applications.Views;

namespace NRules.Samples.ClaimsCenter.Presentation.Views
{
    public partial class MainWindow : IMainView
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public bool IsMaximized
        {
            get { return WindowState == WindowState.Maximized; }
            set
            {
                if (value)
                {
                    WindowState = WindowState.Maximized;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
            }
        }
    }
}