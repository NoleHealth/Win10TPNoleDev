using ByDesign.SOA.Business.Interfaces.[AREA];
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Common.Web.WebAPI.Controllers;
using ByDesign.SOA.Common.Web.WebAPI.Controllers;

namespace BackOffice2.Areas.[AREA].Controllers
{
    /// <summary>
    ///     The main controller for interact with <see cref="I[MODEL]Service" />
    /// </summary>
    [ByDesignOnly]
    public class [MODEL]Controller : ByDesignCRUDController<I[MODEL]>
    {
        public [MODEL]Controller(I[MODEL]Service [mODEL]Service)
        {
            ReadOnlyCRUD([mODEL]Service);
        }
    }
}
