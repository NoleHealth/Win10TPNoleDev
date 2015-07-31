using ByDesign.SOA.Common.IModels;
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Data.Interfaces.[AREA];
using System.Data;

namespace ByDesign.SOA.Data.Repositories.[AREA]
{
    /// <summary>
    ///     The main repository to interact with <see cref="I[MODEL]" />
    /// </summary>
    public class [MODEL]Repository : Repository<I[MODEL]>, I[MODEL]Repository
    {
        //TODO PW#[TICKET]  Only include this Get if description is unique.
        /// <summary>
        ///     Returns a singular <see cref="I[MODEL]" /> for a given description. Returns null if no <see cref="I[MODEL]" /> exists for that ID
        /// </summary>
        /// <param name="description">
        ///     The description of the <see cref="I[MODEL]" /> you'd like
        /// </param>
        /// <returns>
        ///     An <see cref="I[MODEL]" /> for the given description or null if no such <see cref="I[MODEL]" /> exists
        /// </returns>
        public I[MODEL] Get(string description)
        {
            return base.GetFirst(new { description });
        }

        //TODO PW#[TICKET]  Only include if delete supported
        /// <summary>
        ///     Validates a given <see cref="I[MODEL]" /> checking if it is reference by a Lead record
        /// </summary>
        /// <param name="[mODEL]">
        ///     The <see cref="I[MODEL]" /> you'd like validate
        /// </param>
        /// <param name="userName"></param>
        /// <returns>A boolean value if you can delete the <see cref="I[MODEL]" /></returns>
        public IOperationResult ValidateDelete(I[MODEL] [mODEL], string userName)
        {
            dynamic parameters;

            AddParameter("[MODEL]ID", DbType.Int32, [mODEL].ID, initialize: true);
            AddParameter("UserName", DbType.String, userName);
            AddParameter("IsDeletable", DbType.Boolean, paramDirection: ParamDirection.Output);

            base.QueryProcOutputParams("sp[MODEL]_IsDeletable", out parameters);

            bool isDeletable = parameters.Get<bool>("@IsDeletable");

            return Operation(isDeletable);
        }

    }
}