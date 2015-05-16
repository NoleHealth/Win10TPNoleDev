using Windows.UI.Xaml.Controls;

namespace WipHello.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GotoAbout(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            (App.Current as Common.BootStrapper).NavigationService.Navigate(typeof(Views.AboutPage));
        }
    }
}
