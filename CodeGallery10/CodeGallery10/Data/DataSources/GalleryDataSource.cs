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
    public sealed class GalleryDataSource
    {
        private static GalleryDataSource _galleryDataSource = new GalleryDataSource();
        private static readonly object _lock = new object();

        private ObservableCollection<GalleryItemGroup> _groups = new ObservableCollection<GalleryItemGroup>();
        public ObservableCollection<GalleryItemGroup> Groups
        {
            get { return this._groups; }
        }



        public static async Task<IEnumerable<GalleryItemGroup>> GetGroupsAsync()
        {
            await _galleryDataSource.GetGalleryDataAsync();

            return _galleryDataSource.Groups;
        }

        public static async Task<GalleryItemGroup> GetGroupAsync(string uniqueId)
        {
            await _galleryDataSource.GetGalleryDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _galleryDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<GalleryItem> GetItemAsync(string uniqueId)
        {
            await _galleryDataSource.GetGalleryDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _galleryDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() > 0) return matches.First();
            return null;
        }

        private async Task GetGalleryDataAsync()
        {
            lock (_lock)
            {
                if (this._groups.Count != 0)
                    return;
            }
            Uri dataUri;
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                dataUri = new Uri("ms-appx:///Data/DesignTimeData/GalleryData.json");
            else
                dataUri = new Uri("ms-appx:///Data/DemoData/GalleryData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);

            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            lock (_lock)
            {
                foreach (JsonValue groupValue in jsonArray)
                {
                    JsonObject groupObject = groupValue.GetObject();
                    GalleryItemGroup group = new GalleryItemGroup(groupObject["UniqueId"].GetString(),
                                                                          groupObject["Title"].GetString(),
                                                                          groupObject["Subtitle"].GetString(),
                                                                          groupObject["ImagePath"].GetString(),
                                                                          groupObject["Description"].GetString());


                    foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                    {
                        JsonObject itemObject = itemValue.GetObject();
                        var item = new GalleryItem(itemObject["UniqueId"].GetString(),
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
                    if (!this.Groups.Any(g => g.Title == group.Title))
                        this.Groups.Add(group);
                }
            }
        }
    }
}
