using System.Linq;
using System.Waf.Applications;
using System.Windows.Input;
using NRules.Samples.ClaimsCenter.Applications.Views;
using NRules.Samples.ClaimsExpert.Contract;

namespace NRules.Samples.ClaimsCenter.Applications.ViewModels
{
    public class ClaimViewModel : ViewModel<IClaimView>
    {
        private ClaimDto _claim;
        private ICommand _adjudicateCommand;

        public ClaimViewModel(IClaimView view)
            : base(view)
        {
        }

        public ClaimDto Claim
        {
            get { return _claim; }
            set
            {
                if (SetProperty(ref _claim, value))
                {
                    RaisePropertyChanged("IsEnabled");
                    RaisePropertyChanged("HasAlerts");
                }
            }
        }

        public ICommand AdjudicateCommand
        {
            get { return _adjudicateCommand; }
            set { SetProperty(ref _adjudicateCommand, value); }
        }

        public bool IsEnabled { get { return Claim != null; } }
        public bool HasAlerts { get { return Claim != null && Claim.Alerts != null && Claim.Alerts.Any(); } }
    }
}
