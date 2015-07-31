using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign.Excelerator.Classes
{
    public class Model// : ActionableItemBase
    {
        public string Name { get; set; } //"Rep Classification Type"
        public TranslatableText DisplayName { get; set; } //"Rep Classification Types"
        public TranslatableText DisplayNameSingular { get; set; } //"Rep Classification Type"

        public bool IsAuditable { get; set; }
        public Property KeyProperty { get; set; }
        public Property IdentityProperty { get; set; }
        public IList<Property> Properties { get; set; }
       
        public bool IsDeletable { get; set; }
        public TranslatableText HelperText { get; set; }
        public TranslatableText DeleteConfirmation { get; set; }
        public TranslatableText DeleteErrorInUseMessage { get; set; }
        public TranslatableText DeleteErrorReferencesMessage { get; set; }
        public IEnumerable<string> DefaultSort { get; set; }

        public IList<string> DeleteValidationReferences { get; private set; }

        

        //[ByDesignTranslatable("SOA_MODEL_REPCLASSIFICATIONTYPE", "Rep Classification Type")]
        //[ByDesignHelpText("SOA_MODEL_HT_REPCLASSIFICATIONTYPE", "Systematic grouping of Reps according to the structural relationships among them. This grouping can works as promotion condition into the Promotion Engine Module.")]
   
        //ViewBag.DeleteConfirmation = GetTranslation("REPCLASSIFICATIONTYPE_DELETE_CONFIRMATION",
        //                                                "Are you sure you want to delete this Rep Classification Type?");
        //    ViewBag.DeleteErrorMessage = GetTranslation("REPCLASSIFICATIONTYPE_DELETE_ERRORMESSAGE",
        //                                    "Unable to remove record. This type is being used by Rep records.");


        //operationResult.ValidationErrors.Add("ID", "Delete not allowed due to references.");


        

        public Model()
        {
            this.Name = string.Empty;
            this.DisplayName = new TranslatableText();
            this.DisplayNameSingular = new TranslatableText();
 
            this.IsAuditable = false;
            this.Properties = new List<Property>(50);
            this.KeyProperty = null;
            this.IdentityProperty = null;
            
            this.IsDeletable = false;

            this.HelperText = new TranslatableText();
            this.DeleteConfirmation = new TranslatableText();
            this.DeleteErrorInUseMessage = new TranslatableText();
            this.DeleteErrorReferencesMessage = new TranslatableText();

            this.DeleteValidationReferences = new List<string>(10);
            
        }
    }
}
