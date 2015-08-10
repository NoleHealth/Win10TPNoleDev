using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOS.CodeGallery10.Models
{
    public class NavigationItem
    {
        public NavigationItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content
            , String targetUri, String targetParamaters)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Content = content;
            //this.Docs = new ObservableCollection<ControlInfoDocLink>();
            this.RelatedItems = new ObservableCollection<string>();
            this.TargetUri = targetUri;
            this.TargetParamaters = targetParamaters;
        }

        public string TargetUri { get; private set; }
        public string TargetParamaters { get; private set; }
        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Content { get; private set; }
        //public ObservableCollection<ControlInfoDocLink> Docs { get; private set; }
        public ObservableCollection<string> RelatedItems { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
