using ByDesign.SOA.Business.Interfaces.[AREA];
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Data.Interfaces.[AREA];
using System.Collections.Generic;


namespace ByDesign.SOA.Business.Services.[AREA]
{

    /// <summary>
    ///     The main service to interact with <see cref="I[MODEL]" />
    /// </summary>
    public class [MODEL]Service : BaseService, I[MODEL]Service
    {
        /// <summary>
        ///     This is the Cache Key to be used for all references in this service.
        /// </summary>
        private const string CACHE_KEY = "ByDesign.SOA.Business.Services.[AREA].[MODEL]Service";
        private readonly I[MODEL]Repository _[mODEL]Repository;

        /// <summary>
        ///     Creates the new <see cref="I[MODEL]" /> Service. Allows injection of an <see cref="I[MODEL]Repository" />
        /// </summary>
        /// <param name="[mODEL]Repository">
        ///     An implementation of <see cref="I[MODEL]Repository" />
        /// </param>
        public [MODEL]Service(I[MODEL]Repository [mODEL]Repository)
        {
            _[mODEL]Repository = [mODEL]Repository;
            AddTableToUsage("[SQLNAME]");
        }

        /// <summary>
        ///     Gets the list of <see cref="I[MODEL]" /> currently in the system
        /// </summary>
        /// <returns>
        ///     The current list of <see cref="I[MODEL]" /> in the system. You can apply Linq to the result IEnumerable. For instance, a where function
        /// </returns>
        public IEnumerable<I[MODEL]> Get()
        {
            return _[mODEL]Repository.Get();
        }

        /// <summary>
        ///     Returns a singular <see cref="I[MODEL]" /> for a given ID. Returns null if no <see cref="I[MODEL]" /> exists for that ID
        /// </summary>
        /// <param name="id">
        ///     The id of the <see cref="I[MODEL]" /> you'd like
        /// </param>
        /// <returns>
        ///     An <see cref="I[MODEL]" /> for the given ID or null if no such <see cref="I[MODEL]" /> exists
        /// </returns>
        /// <example>
        ///     var example = _[mODEL]Service.Get(1);
        ///     if(example != null)
        ///     Console.WriteLine(example);
        /// </example>
        public I[MODEL] Get(int id)
        {
            return _[mODEL]Repository.Get(id);
        }
    }
}
