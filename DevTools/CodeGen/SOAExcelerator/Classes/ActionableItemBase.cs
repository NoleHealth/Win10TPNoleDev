using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign.Excelerator.Classes
{
    public struct ActionableItem : IActionableItem
    {

        public decimal QuotedDevHours { get; set; }
        public string Status { get; set; }

        //public ActionableItem()
        //{
        //    this.QuotedDevHours = 0;
        //    this.Status = string.Empty;
        //}
        
    }
}
