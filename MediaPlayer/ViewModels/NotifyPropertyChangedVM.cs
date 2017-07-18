using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MediaPlayer.ViewModels
{
    public abstract class NotifyPropertyChangedVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
