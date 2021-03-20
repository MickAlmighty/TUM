using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    /// <summary>
    ///     Generic implementation of RelayCommand. Used for bindings actions with XAML commands.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand
    {
        private Action<T> _Execute
        {
            get;
        }
        private Predicate<T> _CanExecute
        {
            get;
        }

        /// <summary>
        ///     Creates a RelayCommand that executes an action upon receiving a command call.
        /// </summary>
        /// <param name="execute">Action to be executed upon receiving a command call.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null) { }

        /// <summary>
        ///     Creates a RelayCommand that executes an action upon receiving a command call.
        /// </summary>
        /// <param name="execute">Action to be executed upon receiving a command call.</param>
        /// <param name="canExecute">Predicate deciding whether the action can be executed.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute?.Invoke((T)parameter) ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _Execute((T)parameter);
        }
    }

    /// <summary>
    ///     RelayCommand object implementation useful when not dealing with custom arguments.
    /// </summary>
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute) : base(p => execute()) { }

        public RelayCommand(Action execute, Func<bool> canExecute) : base(p => execute(), p => canExecute()) { }
    }
}
