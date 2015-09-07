using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace MOS.CodeGallery10.Mvvm
{
    public abstract class ViewModelBase : BindableBase, Template10.Services.NavigationService.INavigable
    {
        public string Identifier { get; set; }

        public virtual void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state) { /* nothing by default */ }
        public virtual Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending) { return Task.FromResult<object>(null); }
        public virtual void OnNavigatingFrom(Template10.Services.NavigationService.NavigatingEventArgs args) { /* nothing by default */ }

        public NavigationService NavigationService { get; set; }
        public DispatcherWrapper Dispatcher { get { return Template10.Common.WindowWrapper.Current(NavigationService).Dispatcher; } }
    }
}