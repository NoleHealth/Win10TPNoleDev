using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign.Excelerator.Classes
{
    
    public enum PropertyBOAccessType
    {
        NotSet,
        Unknown,
        Editable,
        ReadOnly,
        ReadOnly_AdminOnly,
        EditableByRole_ByDesign

    }
}
