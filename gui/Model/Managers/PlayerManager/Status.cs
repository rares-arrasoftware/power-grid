using System;
using System.Windows;
using System.Windows.Threading;

namespace gui.Model.Managers.PlayerManager
{
    public class Status
    {
        public enum PlayerState
        {
            Active = 0,
            Done,
            Ready,
            Wait
        }

        private PlayerState _state = PlayerState.Wait;
        public PlayerState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStatusChanged();
                }
            }
        }

        private bool _hasCoin = true;
        public bool HasCoin
        {
            get => _hasCoin;
            set
            {
                if (_hasCoin != value)
                {
                    _hasCoin = value;
                    OnStatusChanged();
                }
            }
        }

        // 2. Events
        public event EventHandler? StatusChanged;

        private void OnStatusChanged()
        {
            if (Application.Current?.Dispatcher.CheckAccess() == true)
            {
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Application.Current?.Dispatcher.Invoke(() => StatusChanged?.Invoke(this, EventArgs.Empty));
            }
        }
    }
}
