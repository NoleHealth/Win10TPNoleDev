using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ByDesign.Excelerator.Classes
{
    public class PropertiesColumns
    {
        public static PropertiesColumns Create(DataTable dt)
        {
            PropertiesColumns pc = new PropertiesColumns();

            int headerRow;
            int headerColumn;

            Srd.FindColumn(dt, "Property", out headerRow, out headerColumn);
            pc.Row = headerRow;
            pc.Property = headerColumn;

            Srd.FindColumn(dt, "Display", out headerRow, out headerColumn);
            pc.Display = headerColumn;

            Srd.FindColumn(dt, "Helper Text", out headerRow, out headerColumn);
            pc.HelperText = headerColumn;

            Srd.FindColumn(dt, "UI Type", out headerRow, out headerColumn);
            pc.UIType = headerColumn;

            Srd.FindColumn(dt, "Required", out headerRow, out headerColumn);
            pc.Required = headerColumn;

            Srd.FindColumn(dt, "Default", out headerRow, out headerColumn);
            pc.Default = headerColumn;

            Srd.FindColumn(dt, "Rule", out headerRow, out headerColumn);
            pc.Rule = headerColumn;

            Srd.FindColumn(dt, "System Type", out headerRow, out headerColumn);
            pc.SystemType = headerColumn;

            Srd.FindColumn(dt, "Dependency", out headerRow, out headerColumn);
            pc.Dependency = headerColumn;

            Srd.FindColumn(dt, "DependencyLoading", out headerRow, out headerColumn);
            pc.DependencyLoading = headerColumn;

            Srd.FindColumn(dt, "Char Limit", out headerRow, out headerColumn);
            pc.CharLimit = headerColumn;

            Srd.FindColumn(dt, "CRUD Grid", out headerRow, out headerColumn);
            pc.CRUDGrid = headerColumn;

            Srd.FindColumn(dt, "BO-Access", out headerRow, out headerColumn);
            pc.BOAccess = headerColumn;

            Srd.FindColumn(dt, "API -Access", out headerRow, out headerColumn);
            pc.APIAccess = headerColumn;

            Srd.FindColumn(dt, "Service-Access", out headerRow, out headerColumn);
            pc.ServiceAccess = headerColumn;

            return pc;

        }
        public int Row { get; set; }
        public int Property { get; set; }
        public int Display { get; set; }
        public int HelperText { get; set; }
        public int UIType { get; set; }
        public int Required { get; set; }
        public int Default { get; set; }
        public int Rule { get; set; }
        public int SystemType { get; set; }
        public int Dependency { get; set; }
        public int DependencyLoading { get; set; }
        public int CharLimit { get; set; }
        public int CRUDGrid { get; set; }
        public int BOAccess { get; set; }
        public int APIAccess { get; set; }
        public int ServiceAccess { get; set; }
       
    }
}
