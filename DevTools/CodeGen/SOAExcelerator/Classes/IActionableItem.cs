using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign.Excelerator.Classes
{
    public interface IActionableItem
    {
        decimal QuotedDevHours { get; set;  }
        string Status { get; set; }
        
        
    }
}
