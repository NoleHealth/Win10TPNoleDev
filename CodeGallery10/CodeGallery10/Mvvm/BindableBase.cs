using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Template10.Common;

namespace MOS.CodeGallery10.Mvvm
{
    public abstract class BindableBase : INotifyPropertyChanged, Template10.Mvvm.IBindable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public async void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                return;
            await WindowWrapper.Current().Dispatcher.DispatchAsync(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        public bool Set<T>(ref T storage, T value, [CallerMemberName()]string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
