using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using gui.Model;
using gui.Model.Phases;
using gui.Model.Phases.AuctionPhase;
using gui.Model.Phases.BureaucracyPhase;
using gui.Model.Phases.CityBuildingPhase;
using gui.Model.Phases.ResourceBuyingPhase;
using gui.View.GamePhases;

namespace gui.ViewModel.GamePhases
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
