using System;
using NRules.Samples.ClaimsCenter.Applications.Properties;
using NRules.Samples.ClaimsCenter.Applications.ViewModels;

namespace NRules.Samples.ClaimsCenter.Applications.Controllers
{
    public interface IApplicationController
    {
        void Start();
    }

    internal class ApplicationController : IApplicationController, IDisposable
    {
        private readonly Lazy<MainViewModel> _mainViewModel;
        private readonly IAdjudicationController _adjudicationController;
        private readonly IClaimController _claimController;

        public ApplicationController(Lazy<MainViewModel> mainViewModel, IAdjudicationController adjudicationController, IClaimController claimController)
        {
            _mainViewModel = mainViewModel;
            _adjudicationController = adjudicationController;
            _claimController = claimController;
        }

        public void Start()
        {
            _adjudicationController.Initialize();
            _claimController.Initialize();

            _mainViewModel.Value.Show();
            
            _claimController.Refresh();
        }

        public void Shutdown()
        {
            Settings.Default.Save();
        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}