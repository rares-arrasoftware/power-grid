using System;
using System.Windows.Input;

namespace gui.Helpers
{
    /// <summary>
    /// Simple command implementation for MVVM pattern.
    /// </summary>
    public class RelayCommand : ICommand
    {
        // 1. Fields
        private readonly Action<object?> _execute;

        private readonly Predicate<object?>? _canExecute;

        // 2. Constructors
        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
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
