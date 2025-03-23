using System;
using System.ComponentModel;
using System.Windows.Input;
using gui.Helpers;
using gui.Model.Managers.PlayerManager;

namespace gui.ViewModel.Players
{
    public class ClockViewModel : ViewModelBase
    {
        public Clock Clock;
        public string TimePassed
        {
            get => Clock.TimePassed.ToString(@"hh\:mm\:ss");
            private set
            {
                OnPropertyChanged(nameof(Clock.TimePassed));
            }
        }  

        public ICommand ButtonCommand { get; } 

        public event EventHandler<Clock>? ClockClicked;

        public ClockViewModel(Clock clock)
        {
            Clock = clock;
            ButtonCommand = new RelayCommand(OnButtonClick);
            Clock.TimePassedChanged += (s, e) => OnPropertyChanged(nameof(TimePassed));
        }

        private void OnButtonClick(object? parameter)
        {
            ClockClicked?.Invoke(this, Clock);
        }
    }
}
