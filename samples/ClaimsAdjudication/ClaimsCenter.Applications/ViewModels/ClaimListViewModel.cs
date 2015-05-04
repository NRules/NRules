using System.Collections.ObjectModel;
using System.Waf.Applications;
using NRules.Samples.ClaimsCenter.Applications.Views;
using NRules.Samples.ClaimsExpert.Contract;

namespace NRules.Samples.ClaimsCenter.Applications.ViewModels
{
    public class ClaimListViewModel : ViewModel<IClaimListView>
    {
        private ObservableCollection<ClaimDto> _claims;
        private ClaimDto _selectedClaim;

        public ClaimListViewModel(IClaimListView view) : base(view)
        {
        }

        public ObservableCollection<ClaimDto> Claims
        {
            get { return _claims; }
            set { SetProperty(ref _claims, value); }
        }

        public ClaimDto SelectedClaim
        {
            get { return _selectedClaim; }
            set { SetProperty(ref _selectedClaim, value); }
        }
    }
}
