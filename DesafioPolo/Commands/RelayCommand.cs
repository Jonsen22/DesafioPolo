using System;
using System.Windows.Input;

namespace DesafioPolo
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

#pragma warning disable CS8625 // Não é possível converter um literal nulo em um tipo de referência não anulável.
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
#pragma warning restore CS8625 // Não é possível converter um literal nulo em um tipo de referência não anulável.
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

#pragma warning disable CS8612 // A anulabilidade de tipos de referência em tipo não corresponde ao membro implicitamente implementado.
        public event EventHandler CanExecuteChanged
#pragma warning restore CS8612 // A anulabilidade de tipos de referência em tipo não corresponde ao membro implicitamente implementado.
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
#pragma warning disable CS8604 // Possível argumento de referência nula.
            return _canExecute == null || _canExecute(parameter);
#pragma warning restore CS8604 // Possível argumento de referência nula.
        }

        public void Execute(object? parameter)
        {
#pragma warning disable CS8604 // Possível argumento de referência nula.
            _execute(parameter);
#pragma warning restore CS8604 // Possível argumento de referência nula.
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
