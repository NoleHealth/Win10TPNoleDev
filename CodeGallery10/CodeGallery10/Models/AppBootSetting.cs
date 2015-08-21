using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOS.CodeGallery10.Models
{
    //[Serializable(true)]
    public class AppBootSetting
    {
        public bool Loaded { get; set; } = false;
        public bool ShowAppBackButtonInTitleBar { get; set; } = false;
        public AppBootSetting()
        {
            

        }
    }
}
