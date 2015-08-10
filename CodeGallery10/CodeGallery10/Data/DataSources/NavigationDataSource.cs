using MOS.CodeGallery10.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace MOS.CodeGallery10.Data.DataSources
{
    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// ControlInfoSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class NavigationDataSource
    {
        private static NavigationDataSource _navigationDataSource = new NavigationDataSource();
        private static readonly object _lock = new object();

        private ObservableCollection<NavigationGroup> _navigationGroups = new ObservableCollection<NavigationGroup>();
        public ObservableCollection<NavigationGroup> NavigationGroups
        {
            get { return this._navigationGroups; }
        }



        public static async Task<IEnumerable<NavigationGroup>> GetNavigationGroupsAsync(string app)
        {
            await _navigationDataSource.loadDataAsync(app);

            return _navigationDataSource.NavigationGroups;
        }

        public static async Task<NavigationGroup> GetNavigationGroupAsync(string app, string uniqueId)
        {
            await _navigationDataSource.loadDataAsync(app);
            // Simple linear search is acceptable for small data sets
            var matches = _navigationDataSource.NavigationGroups.Where((g) => g.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<NavigationItem> GetNavigationItemAsync(string app, string uniqueId)
        {
            await _navigationDataSource.loadDataAsync(app);
            // Simple linear search is acceptable for small data sets
            var matches = _navigationDataSource.NavigationGroups.SelectMany(g => g.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() > 0) return matches.First();
            return null;
        }

        private async Task loadDataAsync(string prefix)
        {
            lock (_lock)
            {
                if (this._navigationGroups.Count != 0)
                    return;
            }
            Uri dataUri;
            string fileName;
            if (string.IsNullOrEmpty(prefix))
                fileName = "NavigationData.json";
            else
                fileName = prefix + "NavigationData.json";

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                dataUri = new Uri("ms-appx:///Data/DesignTimeData/" + fileName);
            else
                dataUri = new Uri("ms-appx:///Data/DemoData/" + fileName);

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);

            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            lock (_lock)
            {
                foreach (JsonValue groupValue in jsonArray)
                {
                    JsonObject groupObject = groupValue.GetObject();
                    NavigationGroup group = new NavigationGroup(groupObject["UniqueId"].GetString(),
                                                                          groupObject["Title"].GetString(),
                                                                          groupObject["Subtitle"].GetString(),
                                                                          groupObject["ImagePath"].GetString(),
                                                                          groupObject["Description"].GetString());


                    foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                    {
                        JsonObject itemObject = itemValue.GetObject();
                        var item = new NavigationItem(itemObject["UniqueId"].GetString(),
                                                                itemObject["Title"].GetString(),
                                                                itemObject["Subtitle"].GetString(),
                                                                itemObject["ImagePath"].GetString(),
                                                                itemObject["Description"].GetString(),
                                                                itemObject["Content"].GetString(),
                                                                itemObject["TargetUri"].GetString(),
                                                                itemObject["TargetParamaters"].GetString()
                                                                );
                        //if (itemObject.ContainsKey("Docs"))
                        //{
                        //    foreach (JsonValue docValue in itemObject["Docs"].GetArray())
                        //    {
                        //        JsonObject docObject = docValue.GetObject();
                        //        item.Docs.Add(new ControlInfoDocLink(docObject["Title"].GetString(), docObject["Uri"].GetString()));
                        //    }
                        //}
                        if (itemObject.ContainsKey("RelatedItems"))
                        {
                            foreach (JsonValue relateddItemValue in itemObject["RelatedItems"].GetArray())
                            {
                                item.RelatedItems.Add(relateddItemValue.GetString());
                            }
                        }
                        group.Items.Add(item);
                    }
                    if (!this.NavigationGroups.Any(g => g.Title == group.Title))
                        this.NavigationGroups.Add(group);
                }
            }
        }
    }
}
