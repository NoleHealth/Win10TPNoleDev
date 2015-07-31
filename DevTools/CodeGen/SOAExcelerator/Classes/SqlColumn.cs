using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign.Excelerator.Classes
{
    public class SqlColumn
    {
        //private string _columnName;
        //private string _dataType;
        ////private int? _dataSize;
        //private bool? _nullable;
        //private string _default;
        
        public string ColumnName { get; set; } // { get { return _columnName != null ? _columnName : string.Empty; } set { _columnName = value; } }
        public string DataType { get; set; } // { get { return _dataType != null ? _dataType : string.Empty; } set { _dataType = value; } }
        public int? DataSize { get; set; }
        public bool IsKey { get; set; }
        public bool Nullable { get; set; } // { get { return _nullable.HasValue ? _nullable.Value : false; } set { _nullable = new bool?(false); } }
        public string Default { get; set; } // { get { return _default != null ? _default : string.Empty; } set { _default = value; } }
        public bool IsIdentity { get; set; }

        public SqlColumn() : this("")
        { }
        public SqlColumn(string name)
        {
            ColumnName = name;
            DataType = "";
            DataSize = new int?();
            Nullable = false;
            Default = null;
            IsKey = false;
            IsIdentity = false;
        }
    }
}
