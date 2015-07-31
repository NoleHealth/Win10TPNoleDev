using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign.Excelerator.Classes
{
    public class ServiceAccess// : ActionableItemBase
    {
        public string Get { get; set; }
        public string Create { get; set; }
        public string Update { get; set; }
        public string Delete { get; set; }

        public ServiceAccess()
        {
            this.Get = "No";
            this.Create = "No";
            this.Update = "No";
            this.Delete = "No";
        }
    }
}
