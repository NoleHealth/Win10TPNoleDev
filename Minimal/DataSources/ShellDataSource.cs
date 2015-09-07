using Minimal.Services.SettingsServices;
using Minimal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Minimal.DataSources
{
    public class ShellDataSource
    {
        private static ShellDataSource _ds = new ShellDataSource();
        private static readonly object _lock = new object();

        private ShellViewModel _vm = null;
        public ShellViewModel ShellViewModel { get { return this._vm; } }

        //ShellViewModel
        public static async Task<ShellViewModel> GetShellViewModelAsync(string app)
        {
            await _ds.loadDataAsync(app);

            return _ds.ShellViewModel;
        }

        public static ShellViewModel GetShellViewModel(string app)
        {

            return GetShellViewModelAsync(app).Result;
        }

        private async Task loadDataAsync(string app)
        {
            lock (_lock)
            {
                if (this._vm != null)
                    return;
            }




            _vm = new ShellViewModel();
            _vm.ShowShellBackButton = SettingsService.Instance.UseShellBackButton;


            _vm.CacheMaxDurationDays =SettingsService.Instance.CacheMaxDurationDays;

            _vm.ShowSplashScreen = true; // Factory = (e) => { return new Views.Splash(e); };




            //Uri dataUri;
            //string fileName;
            //if (string.IsNullOrEmpty(app))
            //    fileName = "AppBootSettingData.json";
            //else
            //    fileName = app + "AppBootSettingData.json";

            //if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            //    dataUri = new Uri("ms-appx:///Data/DesignTimeData/" + fileName);
            //else
            //    dataUri = new Uri("ms-appx:///Data/DemoData/" + fileName);

            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            //string jsonText = await FileIO.ReadTextAsync(file);

            ////  "ShowAppBackButtonInTitleBar": false 
            //this.ShellViewModel.ShowShellBackButton = jsonText.Contains("true");




        }
    }
}
