using ByDesign.SOA.Business.Interfaces.[AREA];
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Common.Web.MVC.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace ByDesign.SOA.Applications.WebAPI.Areas.[AREA].Controllers
{
    /// <summary>
    ///     <see cref="[MODEL]Controller" /> Class
    /// </summary>
    public class [MODEL]Controller : ByDesignApiController
    {
        private readonly I[MODEL]Service _[mODEL]Service;

        /// <summary>
        ///     <see cref="[MODEL]Controller" /> Constructor
        /// </summary>
        /// <param name="[mODEL]Service">
        ///     <see cref="I[MODEL]Service" /> used in the controller
        /// </param>
        public [MODEL]Controller(I[MODEL]Service [mODEL]Service)
        {
            _[mODEL]Service = [mODEL]Service;
        }

        // GET api/[MODEL]
        /// <summary>
        ///     Allow you to get a list of  <see cref="I[MODEL]" >Rep Classification Types</see>
        /// </summary>
        /// <returns>
        ///     List of <see cref="I[MODEL]" /> 
        /// </returns>
        [EnableQuery]
        public IEnumerable<I[MODEL]> Get()
        {
            return _[mODEL]Service.Get().AsQueryable();
        }

        // GET api/admin/[MODEL]/{id}
        /// <summary>
        ///     Allows you to get a specific  <see cref="I[MODEL]" >Rep Classification Type</see>
        /// </summary>
        /// <param name="id">
        ///     ID of <see cref="I[MODEL]" /> to get
        /// </param>
        /// <returns>
        ///     <see cref="I[MODEL]" /> with the information
        /// </returns>
        public I[MODEL] Get(int id)
        {
            var response = _[mODEL]Service.Get(id);
            if (response != null)
                return response;

            //if nothing is found, go ahead and throw a NotFound exception
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}
