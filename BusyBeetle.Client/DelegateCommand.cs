using System;
using System.Windows.Input;

namespace BusyBeetle.Client
{
    public delegate bool CommandCanExecute();
    public delegate void Command(object parameter);

    /// <summary>
    /// Helper class to make commands execute methods
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Command _command;
        private readonly CommandCanExecute _canExecute;

        public DelegateCommand(Command command, CommandCanExecute canExecute)
        {
            this._command = command;
            this._canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _command(parameter);
        }
    }
}
