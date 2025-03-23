using gui.Helpers;
using gui.Model;
using System.Windows.Input;

namespace gui.ViewModel.GamePhases
{
    public class BureaucracyViewModel : ViewModelBase
    {
        public BureaucracyViewModel()
        {
            StartRoundCommand = new RelayCommand(StartRound);
        }

        public ICommand StartRoundCommand { get; }

        private void StartRound(object? parameter)
        {
            _ = GameManager.Instance.StartRound();
        }
    }
}
