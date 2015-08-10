using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var g = new GalleryItemGroup("1", "TitleGP", "SubtitleGP", "ImagePathGP", "DescriptionGP");
            g.Items.Add(new GalleryItem("1", "Title", "Subtitle", "ImagePath", "Description", "Content", "TargetUri", "TargetParams"));
            g.Items.Add(new GalleryItem("5", "Title2", "Subtitle2", "ImagePath2", "Description2", "Content2", "TargetUri2", "TargetParams2"));
            g.Items.Add(new GalleryItem("3", "Title3", "Subtitle3", "ImagePath3", "Description3", "Content3", "TargetUri3", "TargetParams3"));


            var g2 = new GalleryItemGroup("2", "TitleGP2", "SubtitleGP2", "ImagePathGP2", "DescriptionGP2");
            g2.Items.Add(new GalleryItem("4", "Title4", "Subtitle4", "ImagePath4", "Description4", "Content4", "TargetUri4", "TargetParams4"));
            g2.Items.Add(new GalleryItem("5", "Title5", "Subtitle5", "ImagePath5", "Description5", "Content5", "TargetUri5", "TargetParams5"));

            GalleryDataSource gg = new GalleryDataSource();
            gg.Groups.Add(g);
            gg.Groups.Add(g2);
            

            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Serialize(gg);
            // Produces string value of: 
            // [ 
            //     {"PersonID":1,"Name":"Bryon Hetrick","Registered":true},
            //     {"PersonID":2,"Name":"Nicole Wilcox","Registered":true},
            //     {"PersonID":3,"Name":"Adrian Martinson","Registered":false},
            //     {"PersonID":4,"Name":"Nora Osborn","Registered":false}
            // ] 

            var deserializedResult = serializer.Deserialize<ObservableCollection<GalleryItemGroup>>(serializedResult);
            // Produces List with 4 Person objects

            int ii = 0;
            ii++;


        }
    }

    public class GalleryItem
    {


      


        public GalleryItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content
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


    public sealed class GalleryDataSource
    {

        private ObservableCollection<GalleryItemGroup> _groups = new ObservableCollection<GalleryItemGroup>();
        public ObservableCollection<GalleryItemGroup> Groups
        {
            get { return this._groups; }
        }


    }
}