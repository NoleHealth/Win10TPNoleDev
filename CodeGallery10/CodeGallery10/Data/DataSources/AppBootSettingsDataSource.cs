using MOS.CodeGallery10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MOS.CodeGallery10.Data.DataSources
{
   

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// ControlInfoSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class AppBootSettingsDataSource
    {
        //{"ShowAppBackButtonInTitleBar":false}
        private static AppBootSettingsDataSource _appBootSettingsDataSource = new AppBootSettingsDataSource();
        private static readonly object _lock = new object();

        private AppBootSetting _appBootSetting = new AppBootSetting();
        public AppBootSetting AppBootSetting { get { return this._appBootSetting; } }
        public static async Task<AppBootSetting> GetAppBootSettingAsync(string app)
        {
            await _appBootSettingsDataSource.loadDataAsync(app);

            return _appBootSettingsDataSource.AppBootSetting;
        }

        public static AppBootSetting GetAppBootSetting(string app)
        {

            return GetAppBootSettingAsync(app).Result;
        }

       
        private async Task loadDataAsync(string app)
        {
            lock (_lock)
            {
                if (this._appBootSetting.Loaded)
                    return;
            }
            _appBootSetting.Loaded = true;

            Uri dataUri;
            string fileName;
            if (string.IsNullOrEmpty(app))
                fileName = "AppBootSettingData.json";
            else
                fileName = app + "AppBootSettingData.json";

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                dataUri = new Uri("ms-appx:///Data/DesignTimeData/" + fileName);
            else
                dataUri = new Uri("ms-appx:///Data/DemoData/" + fileName);

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);

            //  "ShowAppBackButtonInTitleBar": false 
            this.AppBootSetting.ShowAppBackButtonInTitleBar = jsonText.Contains("true");
                


            
        }
    }
}
