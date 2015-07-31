using ByDesign.SOA.Business.Interfaces.[AREA];
using ByDesign.SOA.Common.DI;
using ByDesign.SOA.Common.IModels;
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Common.Providers;
using ByDesign.SOA.Data.Interfaces.[AREA];
using System;
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
        private readonly CRUDHelper<I[MODEL]> _crudHelper;
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
            //looking at this for the first time and wondering why the class doesn't just inherit from a generic abstract class?
            //It's a good question. Read the comments on the CRUDHelper class to better understand why.
            //Any questions/concerns/comments, see the Architect
            _crudHelper = new CRUDHelper<I[MODEL]>([mODEL]Repository, CACHE_KEY, (c) => c.ID);

            AddTableToUsage("[MODEL]s");
        }

        /// <summary>
        ///     Gets the list of <see cref="I[MODEL]" /> currently in the system
        /// </summary>
        /// <returns>
        ///     The current list of <see cref="I[MODEL]" /> in the system. You can apply Linq to the result IEnumerable. For instance, a where function
        /// </returns>
        public IEnumerable<I[MODEL]> Get()
        {
            return _crudHelper.Get();
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
            return _crudHelper.Get(id);
        }

        /// <summary>
        ///     Inserts a new <see cref="I[MODEL]" />
        /// </summary>
        /// <param name="instance">
        ///     The <see cref="I[MODEL]" /> you'd like inserted into the system
        /// </param>
        /// <param name="userName">The user inserting this item</param>
        /// <returns>
        ///     A boolean value if everything went successfully.
        /// </returns>
        public IOperationResult Create(I[MODEL] instance, string userName)
        {
            //TODO PW#[TICKET]  Only use this func if description is unique.
            return VerifyAndReturnOperationIfDescriptionAlreadyExists(instance) ?? _crudHelper.Create(instance, userName);
        }

        /// <summary>
        ///     Updates a given <see cref="I[MODEL]" />
        /// </summary>
        /// <param name="instance">
        ///     The persisted <see cref="I[MODEL]" /> you'd like updated
        /// </param>
        /// <param name="userName">The username authenticated in the system.</param>
        /// <returns>
        ///     A boolean value if everything went successfully
        /// </returns>
        public IOperationResult Update(I[MODEL] instance, string userName)
        {
            //TODO PW#[TICKET]  Only use this func if description is unique.
            return VerifyAndReturnOperationIfDescriptionAlreadyExists(instance) ?? _crudHelper.Update(instance, userName);
        }

        /// <summary>
        ///     Deletes a given <see cref="I[MODEL]" />
        /// </summary>
        /// <param name="instance">
        ///     The <see cref="I[MODEL]" /> you'd like deleted from the system
        /// </param>
        /// <param name="userName">The username authenticated in the system.</param>
        /// <returns>
        ///     A boolean value if everything went successfully
        /// </returns>
        public IOperationResult Delete(I[MODEL] instance, string userName)
        {

            //if all the validators come back with it being ok to delete.
            if (IsDeletable(instance, userName).IsSuccessful)
            {
                return _crudHelper.Delete(instance, userName);
            }

            var operationResult = Container.Instance.Resolve<IOperationResult>();
            operationResult.IsSuccessful = false;
            operationResult.ValidationErrors.Add("ID", "Delete not allowed due to references.");
            return operationResult;
        }

        /// <summary>
        ///     Verifies a given <see cref="I[MODEL]" /> checking that is it not referenced in another table
        /// </summary>
        /// <param name="instance">
        ///     The <see cref="I[MODEL]" /> you'd like verify from the system
        /// </param>
        /// <param name="userName">The username authenticated in the system.</param>
        /// <returns>
        ///     A boolean value indicating if element is deleteable
        /// </returns>
        public IOperationResult IsDeletable(I[MODEL] instance, string userName)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            if (userName == null)
                throw new ArgumentNullException("userName");

            return _[mODEL]Repository.ValidateDelete(instance, userName);
        }

        //TODO PW#[TICKET]  Only need this func if description is unique.
        private IOperationResult VerifyAndReturnOperationIfDescriptionAlreadyExists(I[MODEL] instance)
        {
            if (instance != null)
            {
                var localInstance = _[mODEL]Repository.Get(instance.Description);
                if (localInstance != null && localInstance.ID != instance.ID)
                {
                    var operationResult = Container.Instance.Resolve<IOperationResult>();
                    operationResult.IsSuccessful = false;
                    var translatedError = TranslationProvider.GetTranslation(
                        "SOA_[MODEL]_[mODEL]_DUPLICATEDESCRIPTION",
                        "Cannot have two Rep Classification Types with the same Descriptions.");
                    operationResult.ValidationErrors.Add("Description", translatedError);

                    return operationResult;
                }
            }

            return null;
        }

    }
}