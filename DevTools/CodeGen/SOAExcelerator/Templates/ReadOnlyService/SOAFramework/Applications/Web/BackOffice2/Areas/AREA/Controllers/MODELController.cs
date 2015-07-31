using ByDesign.SOA.Business.Interfaces.[AREA];
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Common.Web.WebAPI.Controllers;

namespace BackOffice2.Areas.[AREA].Controllers
{
    /// <summary>
    ///     The main controller for interact with <see cref="I[MODEL]Service" />
    /// </summary>
    public class [MODEL]Controller : ByDesignCRUDController<I[MODEL]>
    {
        //TODO PW#[TICKET] remove references to MiscFieldTable if this model doesnt have one, otherwise fix the table ID
        private const int MISC_FIELD_TABLE_ID = 17;   

        public [MODEL]Controller(I[MODEL]Service [mODEL]Service)
        {
            ReadOnlyCRUD([mODEL]Service);
        }
    }
}
