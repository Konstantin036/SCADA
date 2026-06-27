using DataConcentrator.Services;
using System.ComponentModel;

namespace ScadaWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAdmin)));
            }
        }

        public void Refresh()
        {
            IsAdmin = UserService.Instance.IsAdmin();
        }
    }
}