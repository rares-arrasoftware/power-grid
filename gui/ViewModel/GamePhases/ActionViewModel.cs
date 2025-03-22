using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using PlayerInput.Model;
using PlayerInput.Model.Phases;
using PlayerInput.Model.Phases.AuctionPhase;
using PlayerInput.Model.Phases.BureaucracyPhase;
using PlayerInput.Model.Phases.CityBuildingPhase;
using PlayerInput.Model.Phases.ResourceBuyingPhase;
using PlayerInput.View.GamePhases;

namespace PlayerInput.ViewModel.GamePhases
{
    public class ActionViewModel : ViewModelBase
    {
        private UserControl _currentPhaseView = new BureaucracyPhaseControl();

        public UserControl CurrentPhaseView
        {
            get => _currentPhaseView;
            set
            {
                _currentPhaseView = value;
                OnPropertyChanged();
            }
        }

        public ActionViewModel()
        {
            GameManager.Instance.PhaseChanged += OnPhaseChanged;
        }


        private void OnPhaseChanged(Phase phase)
        {
            switch (phase)
            {
                case AuctionPhase:
                    CurrentPhaseView = new AuctionPhaseControl();
                    break;
                case ResourceBuyingPhase:
                    CurrentPhaseView = new MarketPhaseControl();
                    break;
                case CityBuildingPhase:
                    CurrentPhaseView = new BuildingPhaseControl();
                    break;
                case BureaucracyPhase:
                    CurrentPhaseView = new BureaucracyPhaseControl();
                    break;
            }
        }
    }
}
