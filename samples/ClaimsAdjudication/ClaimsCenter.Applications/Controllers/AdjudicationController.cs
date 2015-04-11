using System;
using System.Waf.Applications;
using System.Windows.Input;
using Autofac.Features.OwnedInstances;
using NRules.Samples.ClaimsCenter.Applications.ViewModels;
using NRules.Samples.ClaimsExpert.Contract;

namespace NRules.Samples.ClaimsCenter.Applications.Controllers
{
    public interface IAdjudicationController
    {
        void Initialize();
    }

    internal class AdjudicationController : IAdjudicationController
    {
        private readonly Lazy<MainViewModel> _mainViewModel;
        private readonly Lazy<ClaimViewModel> _claimViewModel;
        private readonly Func<Owned<IAdjudicationService>> _adjudicationServiceFactory;
        private readonly IClaimController _claimController;
        private readonly ICommand _adjudicateCommand;

        public AdjudicationController(Lazy<MainViewModel> mainViewModel, Lazy<ClaimViewModel> claimViewModel, 
            Func<Owned<IAdjudicationService>> adjudicationServiceFactory, IClaimController claimController)
        {
            _mainViewModel = mainViewModel;
            _claimViewModel = claimViewModel;
            _adjudicationServiceFactory = adjudicationServiceFactory;
            _claimController = claimController;
            _adjudicateCommand = new DelegateCommand(x => Adjudicate(x as ClaimDto));
        }

        public void Initialize()
        {
            _claimViewModel.Value.AdjudicateCommand = _adjudicateCommand;
        }

        private void Adjudicate(ClaimDto claim)
        {
            using (var service = _adjudicationServiceFactory())
            {
                service.Value.Adjudicate(claim.Id);
            }
            _claimController.RefreshSelected();
        }
    }
}