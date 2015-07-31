using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Data.Interfaces.Application;

namespace ByDesign.SOA.Data.Interfaces.[AREA]
{
    public interface I[MODEL]Repository : ICRUDRepository<I[MODEL]>, IDeleteValidator<I[MODEL]>
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
        I[MODEL] Get(string description);

    }
}