using System;
using System.ComponentModel;
using System.Globalization;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Windows.Input;
using NRules.Samples.ClaimsCenter.Applications.Properties;
using NRules.Samples.ClaimsCenter.Applications.Views;

namespace NRules.Samples.ClaimsCenter.Applications.ViewModels
{
    public class MainViewModel : ViewModel<IMainView>
    {
        private readonly IMessageService _messageService;
        private readonly Lazy<ClaimListViewModel> _claimListViewModel;
        private readonly Lazy<ClaimViewModel> _claimViewModel;
        private readonly ICommand _aboutCommand;

        public MainViewModel(IMainView view, IMessageService messageService, 
            Lazy<ClaimListViewModel> claimListViewModel, Lazy<ClaimViewModel> claimViewModel)
            : base(view)
        {
            _messageService = messageService;
            _claimListViewModel = claimListViewModel;
            _claimViewModel = claimViewModel;
            view.Closing += ViewClosing;
            view.Closed += ViewClosed;

            if (Settings.Default.Left >= 0 && Settings.Default.Top >= 0 && Settings.Default.Width > 0 && Settings.Default.Height > 0)
            {
                ViewCore.Left = Settings.Default.Left;
                ViewCore.Top = Settings.Default.Top;
                ViewCore.Height = Settings.Default.Height;
                ViewCore.Width = Settings.Default.Width;
            }
            ViewCore.IsMaximized = Settings.Default.IsMaximized;

            _aboutCommand = new DelegateCommand(ShowAboutMessage);
        }

        public ICommand AboutCommand { get { return _aboutCommand; } }
        public ICommand RefreshCommand { get; set; }

        public string Title { get { return ApplicationInfo.ProductName; } }

        public object ClaimListView { get { return _claimListViewModel.Value.View; } }
        public object ClaimView { get { return _claimViewModel.Value.View; } }

        public event CancelEventHandler Closing;

        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (Closing != null) { Closing(this, e); }
        }

        private void ViewClosing(object sender, CancelEventArgs e)
        {
            OnClosing(e);
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            Settings.Default.Left = ViewCore.Left;
            Settings.Default.Top = ViewCore.Top;
            Settings.Default.Height = ViewCore.Height;
            Settings.Default.Width = ViewCore.Width;
            Settings.Default.IsMaximized = ViewCore.IsMaximized;
        }

        private void ShowAboutMessage()
        {
            _messageService.ShowMessage(View, string.Format(CultureInfo.CurrentCulture, Resources.AboutText,
                ApplicationInfo.ProductName, ApplicationInfo.Version));
        }
    }
}
