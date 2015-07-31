using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign.Excelerator.Classes
{
    public class Property// : ActionableItemBase
    {

        public string PropertyName { get; set; }
        public string SqlName { get; set; }

        public string ModelType { get; set; }
        public string SqlType { get; set; }


        public int DisplayOrder { get; set; }
        public bool IsIdentityField { get; set; } //IsIdentity
        public bool IsKeyField { get; set; } //KeyField
        
        public bool AdminOnly { get; set; } //[ByDesignAccess]
        public bool Required { get; set; } //[ByDesignRequired]

        public bool IsAudit { get; set; }
        public bool IsInherited { get; set; }
        public int? MaxLength { get; set; } //ByDesignMaxLength

        public SqlColumn SqlColumn { get; set; } //ByDesignMaxLength
        public TranslatableText DisplayName { get; set; }  //[ByDesignTranslatable("SOA_MODEL_REPCLASSIFICATIONTYPE_DESCRIPTION", "Description")]
        public TranslatableText HelperText { get; set; }   //[ByDesignHelpText("SOA_MODEL_HT_REPCLASSIFICATIONTYPE_DESCRIPTION", "The name of the Rep Classification Type.")]


        public TranslatableText DeleteErrorDuplicateMessage { get; set; }

        public PropertyServiceAccessType ServiceAccess { get; set; }
        public PropertyBOAccessType BOAccess { get; set; }
        public PropertyAPIAccessType APIAccess { get; set; }

        public ShowInCrudGridType ShowInCrudGrid { get; set; }
        //var translatedError = TranslationProvider.GetTranslation(
        //                "SOA_MODEL_REPCLASSIFICATIONTYPE_DUPLICATEDESCRIPTION",
        //                "Cannot have two Rep Classification Types with the same Descriptions.");
        //            operationResult.ValidationErrors.Add("Description", translatedError);

        public Property()
        {
            this.PropertyName = "";
            this.SqlName = "";
            this.ModelType = "";
            this.SqlType = "";
            this.IsIdentityField = true;
            this.IsKeyField = false;
            this.AdminOnly = false;
            this.Required = false;
            this.MaxLength = new int?();
            this.DisplayName = new TranslatableText();
            this.HelperText = new TranslatableText();
            this.DeleteErrorDuplicateMessage = new TranslatableText();
            this.IsInherited = false;
            this.IsAudit = false;

            this.BOAccess = PropertyBOAccessType.NotSet;
            this.ServiceAccess = PropertyServiceAccessType.NotSet;
            this.APIAccess = PropertyAPIAccessType.NotSet;
            this.ShowInCrudGrid = ShowInCrudGridType.NotSet;

            this.SqlColumn = null;
            this.DisplayOrder = 0;
        }
     

    }
}
