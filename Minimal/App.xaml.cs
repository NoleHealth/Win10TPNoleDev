using Minimal.Services.SettingsServices;
using Minimal.ViewModels;
using System;
using System.Threading.Tasks;
using Template10;
using Template10.Common;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Minimal.DataSources;

namespace Minimal
{
    sealed partial class App : Template10.Common.BootStrapper
    {
        ShellViewModel _shellViewModel = null;
        public App()
        {
            InitializeComponent();
            _shellViewModel = ShellDataSource.GetShellViewModel("");

            this.CacheMaxDuration = TimeSpan.FromDays(_shellViewModel.CacheMaxDurationDays);
            this.ShowShellBackButton = _shellViewModel.ShowShellBackButton;
            if(_shellViewModel.ShowSplashScreen)
                this.SplashFactory = (e) => { return new Views.Splash(e); };
        }

        // runs even if restored from state
        public override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Window.Current.Content = new Views.Shell(this.FrameFactory(true));
            return base.OnInitializeAsync(args);
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            await Task.Delay(1);

            switch (DecipherStartCause(args))
            {
                case AdditionalKinds.Toast:
                case AdditionalKinds.SecondaryTile:
                    var e = (args as ILaunchActivatedEventArgs);
                    NavigationService.Navigate(typeof(Views.DetailPage), e.Arguments);
                    break;
                default:
                    NavigationService.Navigate(typeof(Views.MainPage));
                    break;
            }
            //return Task.FromResult<object>(null);
        }
    }
}
