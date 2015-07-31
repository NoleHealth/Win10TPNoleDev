using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ByDesign.Excelerator.Classes
{
    public class Srd// : ActionableItemBase
    {
        public string AreaName { get; set; }
        public string CurrentUrl { get; set; }
        public string NewUrl { get; set; }
        public string FilePath { get; set; }

        public string CurrentMenuPath { get; set; }
        public string NewMenuPath { get; set; }

        public Model Model { get; set; }


        public ServiceAccess ServiceAccess { get; set; }
        public ServiceAccess APIAccess { get; set; }
        public ServiceAccess BackOfficeAccess { get; set; }

        public bool UnitTests { get; set; }

        public string PWNumber { get; set; }
        public string Version { get; set; }

        public IList<Rule> Rules { get; set; }
        //public CodeFile? ModelInterfaceFile { get; set; }
        //public CodeFile? ModelFile { get; set; }

        //public CodeFile? ServiceInterfaceFile { get; set; }
        //public CodeFile? ServiceFile { get; set; }

        public IReadOnlyList<CodeFile> CodeFiles { get; set; }


        public IList<SqlColumn> SqlColumns { get; set; }
        public string MiscTableName { get; set; }
        public int? MiscTableID { get; set; } //MISC_FIELD_TABLE_ID
        public string SqlTableName { get; set; } //[Alias("RepClassificationTypes")]
        public Srd()
        {
            this.AreaName = string.Empty;
            this.Model = new Model();
            this.CodeFiles = new CodeFile[0];
            this.CurrentUrl = string.Empty;
            this.NewUrl = string.Empty;
            this.CurrentMenuPath = string.Empty;
            this.NewMenuPath = string.Empty;
            this.UnitTests = false;
            this.ServiceAccess = new ServiceAccess();
            this.APIAccess = new ServiceAccess();
            this.BackOfficeAccess = new ServiceAccess();
            this.PWNumber = string.Empty;
            this.Version = string.Empty;
            this.Rules = new List<Rule>(10);
            this.MiscTableName = string.Empty;
            this.MiscTableID = new int?();
            this.SqlColumns = new List<SqlColumn>(50);
            this.SqlTableName = string.Empty;
            //this.ModelFile = new CodeFile?();
            //this.ModelInterfaceFile = new CodeFile?();
            //this.ServiceInterfaceFile = new CodeFile?();
            //this.ServiceFile = new CodeFile?();
            this.FilePath = string.Empty;

        }


        //internal static Srd Parse(DataSet ds, string sqlTableName)
        //{
        //    return Parse(ds, sqlTableName, string.Empty, 0);
        //}
        //internal static Srd Parse(DataSet ds, string sqlTableName, string sqlMiscTableName, int sqlMiscTableID)
        //{

        internal static Srd Parse(DataSet ds)
        {


            Srd srd = new Srd();



            DataTable dtGeneralInfo = ds.Tables["General Info"];
            DataTable dtServiceDefinition = ds.Tables["Service Definition"];
            DataTable dtProperties = ds.Tables["Properties"];
            DataTable dtRules = ds.Tables["Rules"];
            DataTable dtDDL = ds.Tables["DDL"];





            //fill in srd
            //order matters
            parseGeneralInfo(srd, dtGeneralInfo);
            parseRules(srd, dtRules);
            parseDDL(srd, dtDDL);
            parseServiceDefinition(srd, dtServiceDefinition);
            parseProperties(srd, dtProperties);

            postParseFinalize(srd);
            return srd;

        }

        private static void postParseFinalize(Srd srd)
        {
            var auditableProperties = srd.Model.Properties.Where(p => p.IsAudit == true);
            if (auditableProperties.Count() == 4)
            {
                srd.Model.IsAuditable = true;
                auditableProperties.ToList().ForEach(p => p.IsInherited = true);
            }
            var sqlKeyColumn = srd.SqlColumns.FirstOrDefault(p => p.IsKey == true);
            if (sqlKeyColumn == null)
            {
                sqlKeyColumn = srd.SqlColumns.FirstOrDefault(p => p.ColumnName.Equals("ID", StringComparison.CurrentCultureIgnoreCase));
                if (sqlKeyColumn == null)
                    sqlKeyColumn = srd.SqlColumns.FirstOrDefault(p => p.ColumnName.EndsWith("ID", StringComparison.CurrentCultureIgnoreCase));
                if (sqlKeyColumn == null)
                    sqlKeyColumn = srd.SqlColumns.FirstOrDefault();

                if (sqlKeyColumn != null)
                {
                    sqlKeyColumn.IsKey = true;
                }
            }

            var keyProperty = srd.Model.Properties.FirstOrDefault(p => p.IsKeyField == true);
            if (keyProperty == null)
            {
                keyProperty = srd.Model.Properties.FirstOrDefault(p => p.PropertyName.Equals("ID", StringComparison.CurrentCultureIgnoreCase));
                if (keyProperty == null)
                    keyProperty = srd.Model.Properties.FirstOrDefault(p => p.PropertyName.EndsWith("ID", StringComparison.CurrentCultureIgnoreCase));
                if (keyProperty == null)
                    keyProperty = srd.Model.Properties.FirstOrDefault();

                if (keyProperty != null)
                {
                    keyProperty.IsKeyField = true;
                }
            }


            if (sqlKeyColumn != null && keyProperty != null)
            {
                keyProperty.SqlColumn = sqlKeyColumn;
                //map to property
            }
            //map all properties to sqlColumns
            srd.Model.Properties.Where(p => p.SqlColumn == null).ToList().ForEach(p =>
            {
                p.SqlColumn = srd.SqlColumns.FirstOrDefault(q => q.ColumnName.Equals(p.PropertyName, StringComparison.CurrentCultureIgnoreCase));
            });
            srd.Model.Properties.Where(p => p.SqlColumn == null && p.PropertyName.StartsWith("Is")).ToList().ForEach(p =>
            {
                p.SqlColumn = srd.SqlColumns.FirstOrDefault(q => q.ColumnName.Equals(p.PropertyName.Substring(2), StringComparison.CurrentCultureIgnoreCase));
            });

            srd.Model.Properties.Where(p => p.SqlColumn == null).ToList().ForEach(p =>
            {
                p.SqlColumn = srd.SqlColumns.FirstOrDefault(q => q.ColumnName.Replace("_", "").Equals(p.PropertyName, StringComparison.CurrentCultureIgnoreCase));
            });

            srd.Model.Properties.Where(p => p.SqlColumn != null && p.SqlColumn.DataSize.HasValue && p.MaxLength.HasValue == false).ToList().ForEach(p =>
            {
                p.MaxLength = new int?(p.SqlColumn.DataSize.Value);
            });

            srd.Model.Properties.Where(p => p.SqlColumn != null && string.IsNullOrEmpty(p.SqlName)).ToList().ForEach(p =>
            {
                p.SqlName = p.SqlColumn.ColumnName;
                p.SqlType = p.SqlColumn.DataType;

            });


            var remaining = srd.Model.Properties.Where(p => p.SqlColumn == null);
            if (remaining.Count() > 0)
            {
                int c = remaining.Count();
                string msg = c.ToString() + " model properties could not be mapped to a sql column. Check the IModel and Model file for missing datatype, max length and other attributes." + Environment.NewLine;
                remaining.ToList().ForEach(p => msg += p.PropertyName + Environment.NewLine);

                MessageBox.Show(msg);



            }

            //.ToList().ForEach()
        }


        internal void GenerateAllFiles(string templateFolderName)
        {
            string templateRootFolder;
            if (string.IsNullOrEmpty(templateFolderName))
                templateRootFolder = Path.Combine(Environment.CurrentDirectory, "Templates", "SOAFramework");
            else
                templateRootFolder = Path.Combine(Environment.CurrentDirectory, "Templates", templateFolderName, "SOAFramework");
            if (Directory.Exists(templateRootFolder) == false)
            {
                MessageBox.Show("Directory does not exist: " + templateRootFolder);
                return;
            }
            var templateFiles = Directory.GetFiles(templateRootFolder, "*.*", SearchOption.AllDirectories);
            //Directory.GetFiles(templateRootFolder, "*MODEL*.*", SearchOption.AllDirectories);
            List<CodeFile> codeFiles = new List<CodeFile>(templateFiles.Count());
            foreach (string templatePath in templateFiles)
            {
                string template = File.ReadAllText(templatePath);

                var codeFile = new CodeFile();
                codeFile.Path = templatePath.Substring(templateRootFolder.Length); //remove root
                codeFile.Content = template;

                //replace tokens
                codeFile.Path = replacePathTokens(codeFile.Path);
                codeFile.Content = replaceTemplateTokens(codeFile.Content);

                codeFiles.Add(codeFile);

            }

            this.CodeFiles = codeFiles;


        }


        private static void parseProperties(Srd srd, DataTable dtProperties)
        {

            var pc = PropertiesColumns.Create(dtProperties);
            int displayOrder = 1;
            for (int r = pc.Row + 1; r < dtProperties.Rows.Count; r++)
            {
                //create propert obj from excel 'Properties Tab' row
                var property = parsePropertyDetails(dtProperties.Rows[r], srd.Model.Name, pc);
                if (string.IsNullOrEmpty(property.PropertyName))
                    break;
                srd.Model.Properties.Add(property);
                property.DisplayOrder = displayOrder++;
            }

        }

        private static void parseDDL(Srd srd, DataTable dtDDL)
        {

            //find Column Name
            int maxRow = 10;
            int maxCol = 10;
            int headerRow = 0, headerColumn = 0;

            for (int r = 0; r < maxRow && r < dtDDL.Rows.Count; r++)
            {
                for (int c = 0; c < maxCol; c++)
                {

                    string str = getColumnText(dtDDL.Rows[r], c).Trim().ToLower();
                    if (str == "column name")
                    {
                        headerRow = r;
                        headerColumn = c;
                        break;
                    }

                }

            }
            if (headerRow == 0 || headerColumn == 0)
            {
                MessageBox.Show("Could not parse DDL tab");
                return;

            }

            for (int r = headerRow + 1; r < dtDDL.Rows.Count; r++)
            {
                string sqlColumnName = getColumnText(dtDDL.Rows[r], headerColumn);
                if (string.IsNullOrEmpty(sqlColumnName) == true)
                    break;



                //create propert obj from excel 'Properties Tab' row
                SqlColumn sqlColumn = parseDDLDetails(dtDDL.Rows[r], sqlColumnName, headerColumn);
                srd.SqlColumns.Add(sqlColumn);

            }



        }



        private static void parseRules(Srd srd, DataTable dtRules)
        {
            bool readRowData = false;
            int loopCount = 0;
            List<Rule> rules = new List<Rule>(10);

            foreach (DataRow dr in dtRules.Rows)
            {
                loopCount++;
                if (loopCount > 500)
                {
                    return;
                    ////???
                    //Debug.Fail("Why is the code here?");
                    //break;

                }
                string ruleNumber = getColumnText(dr, 2);
                if (readRowData == false && ruleNumber == "Rule #")
                {
                    //next row will be a property
                    readRowData = true;
                    continue;
                }
                if (readRowData == true && string.IsNullOrEmpty(ruleNumber) == true)
                    //stop on 1st blank row after "Column Name"
                    break;

                if (readRowData == false)
                    continue;

                int ruleNum = int.Parse(ruleNumber);

                string ruleNotes = getColumnText(dr, 3);

                srd.Rules.Add(new Rule() { RuleNum = ruleNum, RuleNote = ruleNotes });



            }
        }

        private static void parseGeneralInfo(Srd srd, DataTable dtGeneralInfo)
        {

            //PW #:
            //Version:
            DataColumn dcCol = dtGeneralInfo.Columns[0];
            DataColumn dcVal = dtGeneralInfo.Columns[1];
            string col;
            foreach (DataRow dr in dtGeneralInfo.Rows)
            {
                if (dr[dcCol] == null && dr[dcCol] is DBNull)
                    continue;
                if (string.IsNullOrEmpty(srd.PWNumber) == false && string.IsNullOrEmpty(srd.Version) == false)
                    break;
                col = dr[dcCol].ToString().Trim();
                if (col == "PW #:")
                    srd.PWNumber = dr[dcVal].ToString().Trim();
                else if (col == "Version:")
                    srd.Version = dr[dcVal].ToString().Trim();
            }

        }

        private static void parseServiceDefinition(Srd srd, DataTable dtServiceDefinition)
        {


            int headerRow;
            int headerColumn;
            FindColumn(dtServiceDefinition, "model", out headerRow, out headerColumn);
            if (headerRow == 0 || headerColumn == 0)
            {
                MessageBox.Show("Could not parse Service Definition tab");
                return;

            }

            srd.MiscTableID = new int?(0);
            DataColumn dcCol = dtServiceDefinition.Columns[headerColumn];
            DataColumn dcVal = dtServiceDefinition.Columns[headerColumn + 1];
            string col, val;
            int propsSet = 0;
            bool readingDeleteValidationReferences = false;
            bool readingService = false;
            bool readingApi = false;
            bool readingBackOffice = false;

            for (int r = headerRow; r < dtServiceDefinition.Rows.Count && r < 500; r++)
            {
                DataRow dr = dtServiceDefinition.Rows[r];

                //foreach (DataRow dr in dtServiceDefinition.Rows)
                //{


                if (dr[dcCol] == null || dr[dcCol] is DBNull || string.IsNullOrWhiteSpace(dr[dcCol].ToString()))
                {
                    if (propsSet >= 20)
                        break;
                    else
                        continue;
                }
                col = dr[dcCol].ToString().Trim();

                if (col.EndsWith(":"))
                    col = col.Substring(0, col.Length - 1);

                if (string.IsNullOrEmpty(col) && readingDeleteValidationReferences == false
                    && readingService == false
                    && readingApi == false
                    && readingBackOffice == false)
                    continue;
                if (readingDeleteValidationReferences && col.Length > 0)
                    readingDeleteValidationReferences = false;

                if (string.IsNullOrEmpty(col))
                {
                    if (readingService)
                        readingService = false;
                    if (readingApi)
                        readingApi = false;
                    if (readingBackOffice)
                        readingBackOffice = false;
                }

                val = dr[dcVal].ToString().Trim();

                switch (col)
                {
                    case "Model":
                        propsSet++;
                        srd.Model.Name = val;
                        break;
                    case "Base Table Name":
                        propsSet++;
                        srd.SqlTableName = val;
                        srd.MiscTableName = val + "Misc";

                        break;
                    case "Display Name":
                        propsSet++;
                        if (string.IsNullOrEmpty(val) == false && val.ToUpper() != "N/A")
                        {
                            srd.Model.DisplayName = new TranslatableText("SOA_MODEL_" + srd.Model.Name.ToUnderscoreAndUpper(), val);
                            if (val.EndsWith("s"))
                            {
                                srd.Model.DisplayNameSingular = new TranslatableText("SOA_MODEL_" + srd.Model.Name.ToUnderscoreAndUpper() + "_PROMPT", val.Substring(0, val.Length - 1));
                            }
                            else
                            {
                                srd.Model.DisplayNameSingular = new TranslatableText("SOA_MODEL_" + srd.Model.Name.ToUnderscoreAndUpper() + "_PROMPT", val);
                            }
                        }
                        break;
                    case "Current URL":
                        propsSet++;
                        srd.CurrentUrl = val;
                        break;
                    case "New URL":
                        propsSet++;
                        srd.NewUrl = val;
                        break;
                    case "Menu Path":
                        propsSet++;
                        srd.CurrentMenuPath = val;
                        break;
                    case "New Menu Path":
                        propsSet++;
                        srd.NewMenuPath = val;
                        break;
                    case "Class Helper Text":
                        propsSet++;
                        srd.Model.HelperText = new TranslatableText("SOA_MODEL_HT_" + srd.Model.Name.ToUpper(), val);
                        break;
                    case "Default Sort":
                        propsSet++;
                        srd.Model.DefaultSort = val.Split(new char[] { ',' }).Select(p => p.Trim());
                        break;
                    case "Delete Validation References":
                        propsSet++;
                        readingDeleteValidationReferences = true;
                        readingService = false;
                        readingApi = false;
                        readingBackOffice = false;
                        srd.Model.DeleteValidationReferences.Clear();
                        break;

                    case "Unit Tests":
                        srd.UnitTests = (val == "Yes") ? true : false;
                        break;
                    case "Service":
                        propsSet++;
                        readingDeleteValidationReferences = false;
                        readingService = true;
                        readingApi = false;
                        readingBackOffice = false;

                        break;
                    case "API":
                        propsSet++;
                        readingDeleteValidationReferences = false;
                        readingService = false;
                        readingApi = true;
                        readingBackOffice = false;

                        break;
                    case "BackOffice":
                        propsSet++;

                        readingDeleteValidationReferences = false;
                        readingService = false;
                        readingApi = false;
                        readingBackOffice = true;

                        break;
                    case "Get":
                        if (readingService)
                            srd.ServiceAccess.Get = val;
                        else if (readingApi)
                            srd.APIAccess.Get = val;
                        else if (readingBackOffice)
                            srd.BackOfficeAccess.Get = val;
                        break;
                    case "Create":
                        if (readingService)
                            srd.ServiceAccess.Create = val;
                        else if (readingApi)
                            srd.APIAccess.Create = val;
                        else if (readingBackOffice)
                            srd.BackOfficeAccess.Create = val;
                        break;
                    case "Update":
                        if (readingService)
                            srd.ServiceAccess.Update = val;
                        else if (readingApi)
                            srd.APIAccess.Update = val;
                        else if (readingBackOffice)
                            srd.BackOfficeAccess.Update = val;
                        break;
                    case "Delete":
                        if (readingService)
                            srd.ServiceAccess.Delete = val;
                        else if (readingApi)
                            srd.APIAccess.Delete = val;
                        else if (readingBackOffice)
                            srd.BackOfficeAccess.Delete = val;
                        break;

                    default:
                        if (readingDeleteValidationReferences && string.IsNullOrEmpty(val) == false)
                        {
                            srd.Model.DeleteValidationReferences.Add(val);
                        }

                        break;
                }

            }

            if(srd.NewUrl.StartsWith("~/") )
            {
                srd.NewUrl = srd.NewUrl.TrimStart('~');
                srd.NewUrl = srd.NewUrl.TrimStart('/');
            }
            else if (srd.NewUrl.StartsWith("/"))
                srd.NewUrl = srd.NewUrl.TrimStart('/');
            else if (srd.NewUrl.StartsWith("~"))
                srd.NewUrl = srd.NewUrl.Trim('~');

            var parts = srd.NewUrl.Split(new char[] { '/' });
            if (parts.Length >= 3)
            {

                srd.FilePath = srd.NewUrl.Split(new char[] {'/'})[parts.Length - 3].FirstCharToUpper() + "/" +
                               srd.NewUrl.Split(new char[] {'/'})[parts.Length - 2].FirstCharToUpper();
            }
            else
            {
                srd.FilePath = srd.NewUrl.Split(new char[] {'/'})[parts.Length - 2].FirstCharToUpper();
            }
            srd.AreaName = srd.FilePath.Replace("/", ".");

        }

        public static void FindColumn(DataTable dt, string col, out int headerRow, out int headerColumn)
        {
            //find Model
            int maxRow = 10;
            int maxCol = 25;
            headerRow = 0;
            headerColumn = 0;
            col = col.ToLower().Replace(" ", "");

            for (int r = 0; r < maxRow && r < dt.Rows.Count; r++)
            {
                for (int c = 0; c < maxCol; c++)
                {

                    string str = getColumnText(dt.Rows[r], c).Trim().ToLower().Replace(" ", ""); ;
                    if (str.StartsWith(col))
                    {
                        headerRow = r;
                        headerColumn = c;
                        return;
                    }

                }

            }
            headerRow = -1;
            headerColumn = -1;
        }


        private static SqlColumn parseDDLDetails(DataRow dr, string sqlColumnName, int startColumn)
        {
            int nameCol, dataTypeCol, nullableCol, defCol, keyCol, identityCol;
            nameCol = startColumn;
            dataTypeCol = startColumn + 1;
            nullableCol = startColumn + 2;
            defCol = startColumn + 3;
            keyCol = startColumn + 4;
            identityCol = startColumn + 5;


            if (string.IsNullOrWhiteSpace(sqlColumnName))
                throw new ArgumentNullException("sqlColumnName");

            var sqlCol = new SqlColumn();
            sqlCol.ColumnName = sqlColumnName;
            sqlCol.DataType = getColumnText(dr, dataTypeCol);
            int p1, p2;
            p1 = sqlCol.DataType.IndexOf(" ident");
            if (p1 > 0)
            {
                //hack int identity(1,1) in datatype column
                sqlCol.DataType = sqlCol.DataType.Substring(0, p1);
            }
            p1 = sqlCol.DataType.IndexOf("(");

            if (p1 > 0)
            {
                p2 = sqlCol.DataType.IndexOf(")", p1);
                if (p2 > 0)
                {
                    string s = sqlCol.DataType.Substring(p1 + 1, p2 - p1 - 1);
                    sqlCol.DataType = sqlCol.DataType.Substring(0, p1);
                    int i = int.Parse(s);
                    if (sqlCol.DataType.Substring(0, 1).ToLower() == "n")
                        sqlCol.DataSize = new int?(i / 2);
                    else
                        sqlCol.DataSize = new int?(i);

                }
            }
            sqlCol.Nullable = getColumnText(dr, nullableCol).ToUpper() == "YES" ? true : false;

            sqlCol.Default = getColumnText(dr, defCol);

            if (sqlCol.Default.ToUpper() == "N/A")
                sqlCol.Default = string.Empty;

            string keyValue = getColumnText(dr, keyCol).Trim().ToLower();
            if (keyValue == "true" || keyValue == "Yes" || keyValue == "primary")
                sqlCol.IsKey = true;

            string identity = getColumnText(dr, identityCol).Trim().ToLower();
            if (identity == "Yes")
                sqlCol.IsIdentity = true;
            else
                sqlCol.IsIdentity = false;

            return sqlCol;
        }

        private static Property parsePropertyDetails(DataRow dr, string modelName, PropertiesColumns pc)
        {

            string propertyName = dr[pc.Property].ToString().Trim();






            //TODO Read a bunch more properties of each property
            var property = new Property() { PropertyName = propertyName };
            string MODEL_UPPER = modelName.ToUnderscoreAndUpper();
            string PROPERTY_UPPER = propertyName.ToUnderscoreAndUpper();

            //DisplayName
            property.DisplayName = new TranslatableText("SOA_MODEL_" + MODEL_UPPER + "_" + PROPERTY_UPPER, dr[pc.Display].ToString().Trim());

            //Helper Text
            property.HelperText = new TranslatableText("SOA_MODEL_HT_" + MODEL_UPPER + "_" + PROPERTY_UPPER, dr[pc.HelperText].ToString().Trim());

            //UI Type


            //Required

            string stringValue = dr[pc.Required].ToString().Trim();
            property.Required = stringValue.Equals("Yes", StringComparison.CurrentCultureIgnoreCase) ? true : false;



            //Default	

            //Rule #


            //System Type

            stringValue = dr[pc.SystemType].ToString().ToLower();
            switch (stringValue)
            {
                case "int":
                    property.ModelType = "int";
                    break;
                case "string":
                    property.ModelType = "string";
                    break;
                case "boolean":
                    property.ModelType = "bool";
                    break;
                case "date":
                    property.ModelType = "DateTime";
                    break;
                case "decimal":
                    property.ModelType = "Decimal";
                    break;
                default:
                    property.ModelType = "Unknown";
                    break;
            }

            if (property.Required == false && property.ModelType != "string")
                property.ModelType += "?";
            //Dependency
            //colOffset++;

            //Dependency Loading
            //colOffset++;

            //Char Limit
            //colOffset++;
            int intValue = 0;
            stringValue = dr[pc.CharLimit].ToString().ToLower();
            if (int.TryParse(stringValue, out intValue))
                property.MaxLength = intValue;
            else
                property.MaxLength = 0;

            //CRUD Grid


            ShowInCrudGridType showInCrudGridTypeValue;


            stringValue = dr[pc.CRUDGrid].ToString();
            if (Enum.TryParse<ShowInCrudGridType>(stringValue, out showInCrudGridTypeValue))
                property.ShowInCrudGrid = showInCrudGridTypeValue;
            else
            {
                if (stringValue.Equals("ADMIN-ONLY", StringComparison.CurrentCultureIgnoreCase))
                    property.ShowInCrudGrid = ShowInCrudGridType.AdminOnly;
                else if (stringValue.Equals("ADMIN ONLY", StringComparison.CurrentCultureIgnoreCase))
                    property.ShowInCrudGrid = ShowInCrudGridType.AdminOnly;
                else if (stringValue.Equals("SHOW", StringComparison.CurrentCultureIgnoreCase))
                    property.ShowInCrudGrid = ShowInCrudGridType.Show;
                //else if (stringValue.Equals("Yes", StringComparison.CurrentCultureIgnoreCase))
                //    property.ShowInCrudGrid = ShowInCrudGridType.Show;
                else if (stringValue.Equals("DO NOT SHOW", StringComparison.CurrentCultureIgnoreCase))
                    property.ShowInCrudGrid = ShowInCrudGridType.DoNotShow;
                //else if (stringValue.Equals("No", StringComparison.CurrentCultureIgnoreCase))
                //    property.ShowInCrudGrid = ShowInCrudGridType.DoNotShow;

                else
                    property.ShowInCrudGrid = ShowInCrudGridType.Unknown;
            }

            //Service-Access

            stringValue = dr[pc.ServiceAccess].ToString();
            PropertyServiceAccessType propertyServiceAccessTypeValue;
            if (Enum.TryParse<PropertyServiceAccessType>(stringValue, out propertyServiceAccessTypeValue))
                property.ServiceAccess = propertyServiceAccessTypeValue;
            else
                property.ServiceAccess = PropertyServiceAccessType.Unknown;

            //BO-Access

            stringValue = dr[pc.BOAccess].ToString();
            PropertyBOAccessType propertyBOAccessTypeValue;

            if (Enum.TryParse<PropertyBOAccessType>(stringValue, out propertyBOAccessTypeValue))
                property.BOAccess = propertyBOAccessTypeValue;
            else
            {
                if (stringValue.Equals("ReadOnly (Admin-only)", StringComparison.CurrentCultureIgnoreCase))
                    property.BOAccess = PropertyBOAccessType.ReadOnly_AdminOnly;
                else if (stringValue.Equals("EditableByRole(\"ByDesign\")"))
                    property.BOAccess = PropertyBOAccessType.EditableByRole_ByDesign;
                else
                    property.BOAccess = PropertyBOAccessType.Unknown;
            }

            //API -Access

            stringValue = dr[pc.APIAccess].ToString();
            if (stringValue.Equals("N/A", StringComparison.CurrentCultureIgnoreCase))
                property.APIAccess = PropertyAPIAccessType.NotApplicable;
            else
            {
                PropertyAPIAccessType propertyAPIAccessTypeValue;
                if (Enum.TryParse<PropertyAPIAccessType>(stringValue, out propertyAPIAccessTypeValue))
                    property.APIAccess = propertyAPIAccessTypeValue;
                else
                    property.APIAccess = PropertyAPIAccessType.Unknown;
            }
            //Numeric Precision
            //property.Numeric Precision = dr[16].ToString();
            //Min Value
            //property.Min Value = dr[17].ToString();
            //Max Value
            //property.Max Value = dr[18].ToString();




            var auditablePropertyNames = new string[] { "DateCreated", "CreatedBy", "DateLastModified", "LastModifiedBy" };

            property.IsAudit = auditablePropertyNames.Any(p => p == property.PropertyName);



            property.DeleteErrorDuplicateMessage = new TranslatableText("", "");

            //srd.Model.Properties.A
            //modelInterfaceFile.Content += Environment.NewLine + "\t\t" + property.ModelType + " " + property.PropertyName + " { get; set; }";


            return property;

        }


        private static string getColumnText(DataRow dr, int colNumber)
        {
            string text;

            if (colNumber < 0 || dr == null || dr.Table.Columns.Count < colNumber + 1 || dr[colNumber] is DBNull || dr[colNumber] == null)
            {
                text = string.Empty;

            }
            else
                text = dr[colNumber].ToString().Trim();

            return text;

        }





        private string replaceTemplateTokens(string content)
        {
            var parts = FilePath.Split(new char[] { '/' });
            string MODEL_UPPER = this.Model.DisplayName.DefaultText.ToUnderscoreAndUpper();
            string newContent = content.Replace("[AREA]", this.AreaName);
            //SQLNAME
            newContent = newContent.Replace("[TICKET]", this.PWNumber);
            newContent = newContent.Replace("[MODEL]", this.Model.Name);
            newContent = newContent.Replace("[MODEL_HT]", this.Model.HelperText.DefaultText);
            newContent = newContent.Replace("[REG]", parts[0]);

            newContent = newContent.Replace("[mODEL]", this.Model.Name.FirstCharToLower());
            newContent = newContent.Replace("[MODEL_DISPLAYNAME]", this.Model.DisplayName.DefaultText);
            newContent = newContent.Replace("[MODEL_DISPLAYNAME_SINGULAR]", this.Model.DisplayNameSingular.DefaultText);

            newContent = newContent.Replace("[MODEL_UPPER]", MODEL_UPPER);
            if (this.Model.IsAuditable)
            {
                newContent = newContent.Replace("#IF IsAuditable #THEN: IAuditable#ENDIF", ": IAuditable");
                newContent = newContent.Replace("#IF IsAuditable #THEN: Auditable,#ENDIF", "Auditable,");
            }
            else
            {
                newContent = newContent.Replace("#IF IsAuditable #THEN: IAuditable#ENDIF", "");
                newContent = newContent.Replace("#IF IsAuditable #THEN: Auditable,#ENDIF", "");
            }

            newContent = newContent.Replace("[TEST_MAX_LENGTH_TYPE]", "NVARCHAR");
            newContent = newContent.Replace("[TEST_MAX_LENGTH_SIZE]", "100");
            newContent = newContent.Replace("[TEST_MAX_LENGTH_PROPERTY]", "Description");

            newContent = newContent.Replace("[SQLNAME]", this.SqlTableName);

            string propString = "";
            if (newContent.Contains("[IMODEL_PROPERTIES]"))
            {
                propString = "";

                foreach (var prop in this.Model.Properties)
                {
                    if (prop.IsInherited == true)
                        continue;
                    propString += Environment.NewLine + "\t\t" + prop.ModelType + " " + prop.PropertyName + " { get; set; }";
                }
                newContent = newContent.Replace("[IMODEL_PROPERTIES]", propString);
            }


            if (newContent.Contains("[MODEL_PROPERTIES]"))
            {
                propString = "";
                foreach (var prop in this.Model.Properties)
                {
                    if (prop.IsInherited == true)
                        continue;

                    propString += Environment.NewLine + "\t\t" + "[DataMember]";

                    if (prop.SqlColumn == null || string.IsNullOrEmpty(prop.SqlColumn.ColumnName))
                    {
                         
                        propString += Environment.NewLine + "\t\t" + "//TODO PW#" + this.PWNumber.ToString() + " Get SQL Column Name";
                        propString += Environment.NewLine + "\t\t" + "[Alias(\"?????\")]";
                    }
                    else
                    {
                        if (prop.SqlColumn.ColumnName.Equals(prop.PropertyName) == false)
                            propString += Environment.NewLine + "\t\t" + "[Alias(\"" + prop.SqlColumn.ColumnName + "\")]";
                    }

                    if (prop.IsKeyField)
                        if (prop.IsIdentityField)
                            propString += Environment.NewLine + "\t\t" + "[KeyField(IsIdentity = true)]";
                        else
                            propString += Environment.NewLine + "\t\t" + "[KeyField(IsIdentity = false)]";

                    if (prop.ShowInCrudGrid == ShowInCrudGridType.AdminOnly)
                    {
                        propString += Environment.NewLine + "\t\t" + "[ByDesignAccess]";
                        propString += Environment.NewLine + "\t\t" + "[ShowOnIndex]";
                    }

                    if (prop.BOAccess == PropertyBOAccessType.EditableByRole_ByDesign || prop.BOAccess == PropertyBOAccessType.ReadOnly_AdminOnly)
                        propString += Environment.NewLine + "\t\t" + "[ByDesignAccess]";

                    if (prop.ShowInCrudGrid == ShowInCrudGridType.Show)
                        propString += Environment.NewLine + "\t\t" + "[ShowOnIndex]";

                    if (prop.Required)
                        propString += Environment.NewLine + "\t\t" + "[ByDesignRequired]";

                    if(prop.ServiceAccess == PropertyServiceAccessType.ReadOnly)
                        propString += Environment.NewLine + "\t\t" + "[ReadOnly(true)]";

                    if (prop.MaxLength.HasValue && prop.MaxLength.Value > 0)
                        propString += Environment.NewLine + "\t\t" + "[ByDesignMaxLength(" + prop.MaxLength.Value.ToString() + ")]";



                    //[Range(1, 365)]


                    if (string.IsNullOrEmpty(prop.DisplayName.DefaultText) == false && prop.DisplayName.DefaultText.Equals("N/A", StringComparison.CurrentCultureIgnoreCase) == false)
                        propString += Environment.NewLine + "\t\t" + "[ByDesignTranslatable(\"" + prop.DisplayName.Key + "\", \"" + prop.DisplayName.DefaultText + "\")]";

                    if (string.IsNullOrEmpty(prop.HelperText.DefaultText) == false && prop.HelperText.DefaultText.Equals("N/A", StringComparison.CurrentCultureIgnoreCase) == false)
                        propString += Environment.NewLine + "\t\t" + "[ByDesignHelpText(\"" + prop.HelperText.Key + "\", \"" + prop.HelperText.DefaultText + "\")]";
                    else
                        propString += Environment.NewLine + "\t\t" + "[ByDesignHelpText(\"" + prop.HelperText.Key + "\", \"\" )]";

                    propString += Environment.NewLine + "\t\tpublic " + prop.ModelType + " " + prop.PropertyName + " { get; set; }";
                    propString += Environment.NewLine;
                }
                newContent = newContent.Replace("[MODEL_PROPERTIES]", propString);
            }
            //[MODEL_CREATE_PROPERTIES]
            if (newContent.Contains("[MODEL_CREATE_PROPERTIES]"))
            {
                propString = "";
                propString += Environment.NewLine + "\t\t" + "//TODO PW#" + this.PWNumber.ToString() + " Some of this is out of date, need to update";
                foreach (var prop in this.Model.Properties)
                {
                    if (prop.IsInherited == true)
                        continue;
                    if (prop.BOAccess == PropertyBOAccessType.ReadOnly_AdminOnly)
                    {
                        propString += "@if (Html.IsByDesignUser())" + Environment.NewLine;
                        propString += "{" + Environment.NewLine;
                        propString += "<div class=\"editor-label bydesign-only\"> " + Environment.NewLine;
                        propString += "@Html.LabelFor(model => model." + prop.PropertyName + ")  " + Environment.NewLine;
                        propString += "@Html.HiddenFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.DisplayTextFor(model => model." + prop.PropertyName + ")" + Environment.NewLine;
                        propString += "@Html.HelpTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "</div>" + Environment.NewLine;
                        propString += "}" + Environment.NewLine;
                    }
                    else if (prop.BOAccess == PropertyBOAccessType.EditableByRole_ByDesign)
                    {
                        propString += "@if (Html.IsByDesignUser())" + Environment.NewLine;
                        propString += "{" + Environment.NewLine;
                        propString += "<div class=\"editor-label bydesign-only\"> " + Environment.NewLine;
                        propString += "@Html.LabelFor(model => model." + prop.PropertyName + ")  " + Environment.NewLine;
                        propString += "@Html.EditorFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.HelpTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.ValidationMessageFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "</div>" + Environment.NewLine;
                        propString += "}" + Environment.NewLine;

                    }
                    else if (prop.BOAccess == PropertyBOAccessType.Editable)
                    {


                        propString += "<div class=\"editor-label\"> " + Environment.NewLine;
                        propString += "@Html.LabelFor(model => model." + prop.PropertyName + ")  " + Environment.NewLine;
                        propString += "@Html.EditorFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.HelpTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.ValidationMessageFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "</div>" + Environment.NewLine;


                    }
                    else if (prop.BOAccess == PropertyBOAccessType.ReadOnly)
                    {
                        propString += "<div class=\"editor-label\"> " + Environment.NewLine;
                        propString += "@Html.LabelFor(model => model." + prop.PropertyName + ")  " + Environment.NewLine;
                        propString += "@Html.DisplayTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.HelpTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "</div>" + Environment.NewLine;
                    }
                    else
                        propString = propString + "";
                }
                if (this.MiscTableID > 0)
                {
                    propString += "<div>@Html.RenderMiscFieldSection(" + this.MiscTableID.ToString() + ")</div>";

                }
                newContent = newContent.Replace("[MODEL_CREATE_PROPERTIES]", propString);

            }
            //[MODEL_EDIT_PROPERTIES]
            if (newContent.Contains("[MODEL_EDIT_PROPERTIES]"))
            {
                propString = "";
                propString += Environment.NewLine + "\t\t" + "//TODO PW#" + this.PWNumber.ToString() + " Some of this is out of date, need to update";
                foreach (var prop in this.Model.Properties)
                {
                    if (prop.IsInherited == true)
                        continue;
                    if (prop.BOAccess == PropertyBOAccessType.ReadOnly_AdminOnly)
                    {
                        propString += "@if (Html.IsByDesignUser())" + Environment.NewLine;
                        propString += "{" + Environment.NewLine;
                        propString += "<div class=\"editor-label bydesign-only\"> " + Environment.NewLine;
                        propString += "@Html.LabelFor(model => model." + prop.PropertyName + ")  " + Environment.NewLine;
                        propString += "@Html.HiddenFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.DisplayTextFor(model => model." + prop.PropertyName + ")" + Environment.NewLine;
                        propString += "@Html.HelpTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "</div>" + Environment.NewLine;
                        propString += "}" + Environment.NewLine;
                    }
                    else if (prop.BOAccess == PropertyBOAccessType.EditableByRole_ByDesign)
                    {
                        propString += "@if (Html.IsByDesignUser())" + Environment.NewLine;
                        propString += "{" + Environment.NewLine;
                        propString += "<div class=\"editor-label bydesign-only\"> " + Environment.NewLine;
                        propString += "@Html.LabelFor(model => model." + prop.PropertyName + ")  " + Environment.NewLine;
                        propString += "@Html.EditorFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.HelpTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.ValidationMessageFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "</div>" + Environment.NewLine;
                        propString += "}" + Environment.NewLine;

                    }
                    else if (prop.BOAccess == PropertyBOAccessType.Editable)
                    {


                        propString += "<div class=\"editor-label\"> " + Environment.NewLine;
                        propString += "@Html.LabelFor(model => model." + prop.PropertyName + ")  " + Environment.NewLine;
                        propString += "@Html.EditorFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.HelpTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.ValidationMessageFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "</div>" + Environment.NewLine;


                    }
                    else if (prop.BOAccess == PropertyBOAccessType.ReadOnly)
                    {
                        propString += "<div class=\"editor-label\"> " + Environment.NewLine;
                        propString += "@Html.LabelFor(model => model." + prop.PropertyName + ")  " + Environment.NewLine;
                        propString += "@Html.DisplayTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "@Html.HelpTextFor(model => model." + prop.PropertyName + ") " + Environment.NewLine;
                        propString += "</div>" + Environment.NewLine;
                    }
                    else
                        propString = propString + "";
                }
                if (this.MiscTableID > 0)
                {
                    propString += "<div>@Html.RenderMiscFieldSection(" + this.MiscTableID.ToString() + ")</div>";

                }
                newContent = newContent.Replace("[MODEL_EDIT_PROPERTIES]", propString);

            }
            //MODEL_INDEX_PROPERTIES
            if (newContent.Contains("[MODEL_INDEX_PROPERTIES]"))
            {
                propString = "";
                foreach (var prop in this.Model.Properties)
                {
                    if (prop.IsInherited == true)
                        continue;
                    if (prop.ShowInCrudGrid == ShowInCrudGridType.DoNotShow)
                        continue;
                    if (!(prop.ShowInCrudGrid == ShowInCrudGridType.Show || prop.ShowInCrudGrid == ShowInCrudGridType.AdminOnly))
                        continue;

                    if (prop.ShowInCrudGrid == ShowInCrudGridType.Show || prop.ShowInCrudGrid == ShowInCrudGridType.AdminOnly)
                    {
                        propString += "columns.Bound(r => r." + prop.PropertyName + ")";
                    }
                    if (prop.ShowInCrudGrid == ShowInCrudGridType.AdminOnly)
                        propString += ".Visible(Html.IsByDesignUser())";

                    propString += ";" + Environment.NewLine;
                    /*
                     columns.Bound(r => r.ID).Visible(Html.IsByDesignUser());
                columns.Bound(r => r.Description);
                columns.Bound(r => r.Explanation);
                     */

                }
                newContent = newContent.Replace("[MODEL_INDEX_PROPERTIES]", propString);
            }
            if (newContent.Contains("[SORT]"))
            {
                propString = "";
                if (this.Model.DefaultSort.Count() == 0)
                {
                    propString = "";

                }
                else
                {
                    propString = ".Sort(sort =>" + Environment.NewLine;
                    propString += "{" + Environment.NewLine;
                    var list = this.Model.DefaultSort.ToList();
                    list.ForEach(p =>
                    {
                        propString += "sort.Add(r => r." + p + ");" + Environment.NewLine;
                    });
                    propString += "})" + Environment.NewLine;
                }
                newContent = newContent.Replace("[SORT]", propString);



            }


            return newContent;
        }


        private string replacePathTokens(string templateFilePath)
        {
            var parts = FilePath.Split(new char[] { '/' });
            return
                templateFilePath.Replace("AREA", this.FilePath)
                    .Replace("MODEL", this.Model.Name)
                    .Replace("TICKET", this.PWNumber)
                    .Replace("REG", parts[0]);
        }

        internal void ClearFolder(string rootPath)
        {
            Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories).ToList().ForEach(f => tryDeleteFile(f));
            Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories).ToList().ForEach(d => tryDeleteFolder(d));
        }
        internal void WriteFiles(string rootPath)
        {

            


            foreach (var codeFile in this.CodeFiles)
            {
                string fullPath = rootPath + codeFile.Path;
                //if(fullPath.Split('\\').Any(p => p.Equals("MODEL", StringComparison.CurrentCulture)))
                //{
                //    fullPath.Replace("\\MODEL\\", this.Model.Name
                //}
                string dir = Path.GetDirectoryName(fullPath);
                if (Directory.Exists(dir) == false)
                    Directory.CreateDirectory(dir);

                File.WriteAllText(fullPath, codeFile.Content);

            }
        }

        private void tryDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch { }
        }

        private void tryDeleteFolder(string path)
        {
            try
            {
                Directory.Delete(path);
            }
            catch { }
        }
    }



    //internal static Srd ParseOld(DataSet ds)
    //{
    //    Srd srd = new Srd();


    //    DataTable dtServiceDefinition = ds.Tables["Service Definition"];
    //    DataTable dtProperties = ds.Tables["Properties"];


    //    bool read = false;
    //    bool hasDateCreated = false;
    //    bool hasCreatedBy = false;
    //    bool hasDateLastModified = false;
    //    bool hasLastModifiedBy = false;



    //    foreach (DataRow dr in dtServiceDefinition.Rows)
    //    {
    //        if (string.IsNullOrEmpty(srd.Model.Name) == false && string.IsNullOrEmpty(srd.AreaName) == false)
    //            break;
    //        if (dr[2].ToString() == "Model:")
    //        {
    //            srd.Model.Name = dr[3].ToString();
    //            continue;
    //        }
    //        if (dr[2].ToString() == "New URL:")
    //        {
    //            string areaName = dr[3].ToString().Split(new char[] { '/' })[1];
    //            srd.AreaName = areaName.Substring(0, 1).ToUpper() + areaName.Substring(1);
    //            continue;
    //        }

    //    }

    //    List<DataRow> codeRows = new List<DataRow>(dtProperties.Rows.Count);

    //    foreach (DataRow dr in dtProperties.Rows)
    //    {
    //        string prop = dr[3].ToString();
    //        if (read == false && prop == "Property")
    //        {
    //            read = true;
    //            continue;
    //        }
    //        if (read == true && string.IsNullOrEmpty(prop) == true)
    //            break;

    //        if (read == false)
    //            continue;

    //        codeRows.Add(dr);
    //        if (prop == "DateCreated")
    //            hasDateCreated = true;
    //        if (prop == "CreatedBy")
    //            hasCreatedBy = true;
    //        if (prop == "DateLastModified")
    //            hasDateLastModified = true;
    //        if (prop == "LastModifiedBy")
    //            hasLastModifiedBy = true;


    //    }
    //    if (hasCreatedBy && hasDateCreated && hasDateLastModified && hasLastModifiedBy)
    //        srd.Model.IsAuditable = true;



    //    var modelInterfaceFile = new CodeFile();

    //    modelInterfaceFile.Content = "";
    //    modelInterfaceFile.Content += Environment.NewLine + "namespace ByDesign.SOA.Common.IModels." + srd.AreaName;
    //    modelInterfaceFile.Content += Environment.NewLine + "{";

    //    if (srd.Model.IsAuditable)
    //        modelInterfaceFile.Content += Environment.NewLine + "\tpublic interface I" + srd.Model.Name + " : IAuditable";
    //    else
    //        modelInterfaceFile.Content += Environment.NewLine + "\tpublic interface I" + srd.Model.Name;
    //    modelInterfaceFile.Content += Environment.NewLine + "\t{";

    //    foreach (DataRow dr in codeRows)
    //    {
    //        string prop, type, required;
    //        prop = dr[3].ToString();
    //        type = dr[11].ToString();
    //        required = dr[8].ToString();
    //        string typePost;
    //        if (required == "Yes")
    //        {
    //            typePost = "";
    //        }
    //        else
    //        {
    //            typePost = "?";
    //        }
    //        if (srd.Model.IsAuditable && (prop == "DateCreated" || prop == "CreatedBy" || prop == "DateLastModified" || prop == "LastModifiedBy"))
    //            continue;
    //        switch (type)
    //        {
    //            case "Int":
    //                type = "int";
    //                break;
    //            case "String":
    //                type = "string";
    //                break;
    //            case "Boolean":
    //                type = "bool";
    //                break;
    //            case "Date":
    //                type = "DateTime";
    //                break;

    //        }
    //        Property property = new Property();
    //        property.DeleteErrorDuplicateMessage = new TranslatableText("", "");

    //        //srd.Model.Properties.A
    //        modelInterfaceFile.Content += Environment.NewLine + "\t\t" + type + typePost + " " + prop + " { get; set; }";
    //    }


    //    modelInterfaceFile.Content += Environment.NewLine + "\t}";
    //    modelInterfaceFile.Content += Environment.NewLine + "}";



    //    //string trunkFolder = @"C:\CodeSoa\trunk\";


    //    modelInterfaceFile.Path = /*trunkFolder +*/ @"\Common\ByDesign.Common.IModels\" + srd.AreaName + @"\I" + srd.Model.Name + ".cs";
    //    srd.ModelInterfaceFile = new CodeFile?(modelInterfaceFile);

    //    return srd;

    //}


    //private void createModelInterface()
    //{
    //    string modelName = "";
    //    string areaName = "";

    //    string code = "";
    //    bool read = false;
    //    bool hasDateCreated = false;
    //    bool hasCreatedBy = false;
    //    bool hasDateLastModified = false;
    //    bool hasLastModifiedBy = false;
    //    bool isIAuditable = false;

    //    var dt = ds.Tables["Service Definition"];
    //    foreach (DataRow dr in dt.Rows)
    //    {
    //        if (string.IsNullOrEmpty(modelName) == false && string.IsNullOrEmpty(areaName) == false)
    //            break;
    //        if (dr[2].ToString() == "Model:")
    //        {
    //            modelName = dr[3].ToString();
    //            continue;
    //        }
    //        if (dr[2].ToString() == "New URL:")
    //        {
    //            areaName = dr[3].ToString().Split(new char[] { '/' })[1];
    //            areaName = areaName.Substring(0, 1).ToUpper() + areaName.Substring(1);
    //            continue;
    //        }

    //    }
    //    dt = ds.Tables["Properties"];
    //    List<DataRow> codeRows = new List<DataRow>(dt.Rows.Count);

    //    foreach (DataRow dr in dt.Rows)
    //    {
    //        string prop = dr[3].ToString();
    //        if (read == false && prop == "Property")
    //        {
    //            read = true;
    //            continue;
    //        }
    //        if (read == true && string.IsNullOrEmpty(prop) == true)
    //            break;

    //        if (read == false)
    //            continue;

    //        codeRows.Add(dr);
    //        if (prop == "DateCreated")
    //            hasDateCreated = true;
    //        if (prop == "CreatedBy")
    //            hasCreatedBy = true;
    //        if (prop == "DateLastModified")
    //            hasDateLastModified = true;
    //        if (prop == "LastModifiedBy")
    //            hasLastModifiedBy = true;


    //    }
    //    if (hasCreatedBy && hasDateCreated && hasDateLastModified && hasLastModifiedBy)
    //        isIAuditable = true;

    //    code = code + Environment.NewLine + "namespace ByDesign.SOA.Common.IModels." + areaName;
    //    code = code + Environment.NewLine + "{";

    //    if (isIAuditable)
    //        code = code + Environment.NewLine + "\tpublic interface I" + modelName + " : IAuditable";
    //    else
    //        code = code + Environment.NewLine + "\tpublic interface I" + modelName;
    //    code = code + Environment.NewLine + "\t{";

    //    foreach (DataRow dr in codeRows)
    //    {
    //        string prop, type, required;
    //        prop = dr[3].ToString();
    //        type = dr[11].ToString();
    //        required = dr[8].ToString();
    //        string typePost;
    //        if (required == "Yes")
    //        {
    //            typePost = "";
    //        }
    //        else
    //        {
    //            typePost = "?";
    //        }
    //        if (isIAuditable && (prop == "DateCreated" || prop == "CreatedBy" || prop == "DateLastModified" || prop == "LastModifiedBy"))
    //            continue;
    //        switch (type)
    //        {
    //            case "Int":
    //                type = "int";
    //                break;
    //            case "String":
    //                type = "string";
    //                break;
    //            case "Boolean":
    //                type = "bool";
    //                break;
    //            case "Date":
    //                type = "DateTime";
    //                break;

    //        }
    //        code = code + Environment.NewLine + "\t\t" + type + typePost + " " + prop + " { get; set; }";
    //    }


    //    code = code + Environment.NewLine + "\t}";
    //    code = code + Environment.NewLine + "}";

    //    code += "";
    //    //string cnString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';";
    //    //DataSet ds = new DataSet();
    //    //using (OleDbConnection cn = new OleDbConnection(cnString))
    //    //using (OleDbCommand cmd = new OleDbCommand())
    //    //using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
    //    //{
    //    //    cmd.Connection = cn;
    //    //    cmd.CommandType = CommandType.Text;
    //    //    cmd.CommandText = "SELECT * FROM [Sheet1$]";
    //    //    cn.Open();
    //    //    da.Fill(ds);

    //    //}

    //    string trunkFolder = @"C:\CodeSoa\trunk\";

    //    string newFile = trunkFolder + @"Common\ByDesign.Common.IModels\" + areaName + @"\I" + modelName + ".cs";

    //    int i;
    //    i = 0;
    //    i++;
    //    //Extended Properties=\"Excel 8.0;HDR=YES;\"";
    //}


}
