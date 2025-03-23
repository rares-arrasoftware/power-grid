using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace gui.Model.Managers.InfoManager
{
    public class InfoManager : INotifyPropertyChanged
    {
        private static readonly Lazy<InfoManager> _instance =
            new Lazy<InfoManager>(() => Application.Current.Dispatcher.Invoke(() => new InfoManager()));

        public static InfoManager Instance => _instance.Value;

        public event PropertyChangedEventHandler? PropertyChanged;

        private InfoManager() { }

        private void SetProperty(Action updateField, string propertyName)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => SetProperty(updateField, propertyName));
                return;
            }
            updateField();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Application.Current.Dispatcher.InvokeAsync(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)), DispatcherPriority.Render);
        }

        private string _phaseName = "N/A";
        public string PhaseName { get => _phaseName; set => SetProperty(() => _phaseName = value, nameof(PhaseName)); }

        private string _playerName = "N/A";
        public string PlayerName { get => _playerName; set => SetProperty(() => _playerName = value, nameof(PlayerName)); }

        private string _optionA = "N/A";
        public string OptionA { get => _optionA; set => SetProperty(() => _optionA = value, nameof(OptionA)); }

        private string _optionB = "N/A";
        public string OptionB { get => _optionB; set => SetProperty(() => _optionB = value, nameof(OptionB)); }

        private string _optionC = "N/A";
        public string OptionC { get => _optionC; set => SetProperty(() => _optionC = value, nameof(OptionC)); }

        private string _optionD = "N/A";
        public string OptionD { get => _optionD; set => SetProperty(() => _optionD = value, nameof(OptionD)); }
    }
}
