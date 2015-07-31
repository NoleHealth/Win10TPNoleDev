using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign.Excelerator.Classes
{
    public class Rule// : ActionableItemBase
    {
        public int RuleNum { get; set; }
        public string RuleNote { get; set; }
        

        public Rule()
        {
            this.RuleNum = 0;
            this.RuleNote = string.Empty;
        }

      
    }
}
