using PlayerInput.Helpers;
using PlayerInput.Model.Managers.PlayerManager;
using PlayerInput.View.Players;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PlayerInput.ViewModel.Players
{
    class PlayerPanelViewModel : ViewModelBase
    {
        // 1. Properties
        public Player Player { get; }

        public string Name => Player.Name;

        public StatusBtnViewModel Status { get; }

        public ClockViewModel Clock { get; }

        public ICommand DoubleClickCommand { get; }

        // 2. Events
        public event EventHandler<Clock>? ClockClicked;

        public event EventHandler<Status>? StatusClicked;

        // 3. Consturctors
        public PlayerPanelViewModel(Player player)
        {
            Player = player;
            Status = new StatusBtnViewModel(player.Status);
            Status.StatusClicked += OnStatusClicked;
            Clock = new ClockViewModel(player.Clock);
            Clock.ClockClicked += OnClockClicked;
            DoubleClickCommand = new RelayCommand(_ => OnDoubleClick());
            Player.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Player.Name))
                {
                    OnPropertyChanged(nameof(Name));
                }
            };
        }

        // 4. Private Methods
        private void OnStatusClicked(object? sender, Status status)
        {
            StatusClicked?.Invoke(this, status);
        }

        private void OnClockClicked(object? sender, Clock clock)
        {
            ClockClicked?.Invoke(this, clock);
        }

        private void OnDoubleClick()
        {
            var dialog = new PlayerDetailsDialog
            {
                DataContext = new PlayerDetailsViewModel(Player) // assuming "this" is Player
            };

            dialog.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            dialog.ShowDialog();
        }

    }
}
