using System;
using System.Windows.Input;

namespace gui.Helpers
{
    public class RelayCommand : ICommand
    {
        // 1. Fields
        private readonly Action<object?> _execute;

        private readonly Predicate<object?>? _canExecute;

        // 2. Constructors
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // 3. ICommand Implementation
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        // 4. Events
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
