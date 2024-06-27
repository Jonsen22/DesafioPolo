using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesafioPolo.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
#pragma warning disable CS8612 // A anulabilidade de tipos de referência em tipo não corresponde ao membro implicitamente implementado.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8612 // A anulabilidade de tipos de referência em tipo não corresponde ao membro implicitamente implementado.
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.

#pragma warning disable CS8625 // Não é possível converter um literal nulo em um tipo de referência não anulável.
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
#pragma warning restore CS8625 // Não é possível converter um literal nulo em um tipo de referência não anulável.
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

#pragma warning disable CS8625 // Não é possível converter um literal nulo em um tipo de referência não anulável.
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
#pragma warning restore CS8625 // Não é possível converter um literal nulo em um tipo de referência não anulável.
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
