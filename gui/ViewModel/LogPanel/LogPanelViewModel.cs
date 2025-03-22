using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlayerInput.ViewModel.LogWindow
{
    public class LogPanelViewModel : ViewModelBase
    {
        public ObservableCollection<string> Logs { get; } = new();

        public void Add(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Logs.Add(message);
                OnPropertyChanged(nameof(Logs));
            });
        }
    }
}
