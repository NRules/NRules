using System;
using System.Waf.Applications;
using NRules.Samples.ClaimsCenter.Applications.ViewModels;
using NRules.Samples.ClaimsCenter.Applications.Views;

namespace NRules.Samples.ClaimsCenter.Presentation.Views
{
    public partial class ClaimListView : IClaimListView
    {
        private readonly Lazy<ClaimListViewModel> _viewModel;

        public ClaimListView()
        {
            InitializeComponent();

            _viewModel = new Lazy<ClaimListViewModel>(this.GetViewModel<ClaimListViewModel>);
        }

        private ClaimListViewModel ViewModel
        {
            get { return _viewModel.Value; }
        }
    }
}