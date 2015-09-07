using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Template10.Common;
using MOS.CodeGallery10.Models;
using MOS.CodeGallery10.Data.DataSources;

namespace MOS.CodeGallery10
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : BootStrapper
    {
        AppBootSetting _appBootSettings = null;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 
        public App()
        {

            InitializeComponent();
            _appBootSettings = AppBootSettingsDataSource.GetAppBootSetting("");

            this.ShowShellBackButton = _appBootSettings.ShowAppBackButtonInTitleBar;

        }

       



        public override Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            if (startKind == StartKind.Launch)
            {
                this.NavigationService.Navigate(typeof(Views.MainPage));
            }
            else
            {
                // this.NavigationService.Navigate(typeof(Views.SecondPage));
            }
            return Task.FromResult<object>(null);
        }
    }
}
