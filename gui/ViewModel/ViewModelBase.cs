using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace gui.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        // 1. Events
        public event PropertyChangedEventHandler? PropertyChanged;

        // 2. Protected Methods
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            // Check if the new value is different from the current value
            if (Equals(storage, value))
                return false;

            // Update the backing field and raise the PropertyChanged event
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
