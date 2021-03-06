﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOS.CodeGallery10.Models
{
    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class GalleryItemGroup
    {

        

        public GalleryItemGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Items = new ObservableCollection<GalleryItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<GalleryItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
