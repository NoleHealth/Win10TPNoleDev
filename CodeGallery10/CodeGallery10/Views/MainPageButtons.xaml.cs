using MOS.CodeGallery10.ViewModels;
using Windows.UI.Xaml.Controls;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MOS.CodeGallery10.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPageButtons : Page
    {
        // strongly-typed view models enable x:bind
        public MainPageViewModel ViewModel { get { return this.DataContext as MainPageViewModel; } }

        public MainPageButtons()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
    }
}
