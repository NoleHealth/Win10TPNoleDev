using System.Reflection;
using System.Web.Mvc;
using ByDesign.SOA.Business.Interfaces.[AREA];
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Common.Web.WebAPI.Controllers;
using ByDesign.SOA.Common.Web.WebAPI.Filters;
using ByDesign.SOA.Common.Web.WebAPI.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;

namespace BackOffice2.Areas.[AREA].Controllers
{
    /// <summary>
    ///     The main controller for interact with <see cref="I[MODEL]Service" />
    /// </summary>
    public class [MODEL]Controller : ByDesignController
    {
        //TODO PW#[TICKET]  Fix Table ID or Remove if no Misc Table
        private const int MISC_FIELD_TABLE_ID = 17;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly I[MODEL]Service _[mODEL]Service;

        public [MODEL]Controller(I[MODEL]Service [mODEL]Service)
        {
            
            _[mODEL]Service = [mODEL]Service;
            SqlTableAudit(_[mODEL]Service.TablesUsage);
        }

        /// <summary>
        /// Returns the Index View. Data will be retrieved via AJAX
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            Log.Debug("Index retrieved");
            ViewBag.Title = GetTranslation("SOA_MODEL_[MODEL_UPPER]", "[MODEL_DISPLAYNAME]");
            return View();
        }

        /// <summary>
        ///     Returns the Data for the AJAX request. Data is handled (sorted) using ToDataSourceResult
        /// </summary>
        /// <param name="request">
        ///     Data Source Request that ask for the <see cref="I[MODEL]" /> List
        /// </param>
        /// <returns>
        ///     Json result for the request
        /// </returns>
        public JsonResult Get([DataSourceRequest] DataSourceRequest request)
        {
            Log.Debug("Getting data");
            var data = _[mODEL]Service.Get();
            Log.Debug("Returning Data");
            ViewBag.Yes = GetTranslation("YES", "Yes");
            ViewBag.No = GetTranslation("NO", "No");
            //the ToDataSourceResult uses the request information to appropriately apply filters
            return Json(data.ToDataSourceResult(request));
        }
        
        /// <summary>
        ///     Get Method to create a new <see cref="I[MODEL]" />
        /// </summary>
        /// <returns>
        ///     Action result with the <see cref="I[MODEL]" /> base to create
        /// </returns>
        [HttpGet]
        [MiscFieldActionFilter(MISC_FIELD_TABLE_ID)]
        public ActionResult Create()
        {
            //We get a "FRESH" instance of a model to pass. The view expects a model as a post to create could have model errors and they would be in the model.
            var [mODEL] = ByDesign.SOA.Common.DI.Container.Instance.Resolve<I[MODEL]>();
            ViewBag.Title = string.Format("{0} - {1}", GetTranslation("SOA_MODEL_[MODEL_UPPER]", "[MODEL_DISPLAYNAME]"), GetTranslation("CREATE", "Create"));
            return View([mODEL]);
        }
        
        /// <summary>
        ///     Post Create Method to save a new <see cref="I[MODEL]" />
        /// </summary>
        /// <param name="[mODEL]Entry">
        ///     <see cref="I[MODEL]" /> with the information to save
        /// </param>
        /// <returns>
        ///     Fine - Redirect to the Index View
        ///     Not Fine - Action Result with the <see cref="I[MODEL]" /> to create
        /// </returns>
        [HttpPost]
        [MiscFieldActionFilter(MISC_FIELD_TABLE_ID)]
        public ActionResult Create(I[MODEL] [mODEL]Entry)
        {
            var [mODEL] = ByDesign.SOA.Common.DI.Container.Instance.Resolve<I[MODEL]>();
            TryUpdateModel([mODEL]);

            if (ModelState.IsValid)
            {
                _[mODEL]Service.Create([mODEL], User.Identity.Name);

                // Need the ID of the newly-created record to save the MiscFields.
                HttpContext.Items[HtmlHelperExtensions.GetMiscFieldPrefix(MISC_FIELD_TABLE_ID) + "id"] = [mODEL].ID;

                return RedirectToAction("Index");
            }
            ViewBag.Title = string.Format("{0} - {1}", GetTranslation("SOA_MODEL_[MODEL_UPPER]", "[MODEL_DISPLAYNAME]"), GetTranslation("CREATE", "Create"));
            return View([mODEL]);
        }

        /// <summary>
        ///     Get Edit method to Edit a <see cref="I[MODEL]" />
        /// </summary>
        /// <param name="id">
        ///     Id of <see cref="I[MODEL]" /> to Edit
        /// </param>
        /// <returns>
        ///     Action result with the <see cref="I[MODEL]" /> to Edit
        /// </returns>
        [HttpGet]
        [MiscFieldActionFilter(MISC_FIELD_TABLE_ID)]
        public ActionResult Edit(int id)
        {
            var [mODEL] = _[mODEL]Service.Get(id);

            Throw404IfModelIsNotFound([mODEL]);

            SetupEditViewBagItems([mODEL]);

            return View([mODEL]);
        }

        /// <summary>
        ///     Post Edit method to save the changes in <see cref="I[MODEL]" />
        /// </summary>
        /// <param name="id">
        ///      Id of <see cref="I[MODEL]" /> to Edit
        /// </param>
        /// <param name="[mODEL]Entry">
        ///      <see cref="I[MODEL]" />  with the new values to save
        /// </param>
        /// <returns>
        ///     Fine - Redirect to the Index View
        ///     Not Fine - Action Result with the <see cref="I[MODEL]" /> to Edit
        /// </returns
        [HttpPost]
        [MiscFieldActionFilter(MISC_FIELD_TABLE_ID)]
        public ActionResult Edit(int id, I[MODEL] [mODEL]Entry, FormCollection formCollection)
        {
            var [mODEL] = _[mODEL]Service.Get(id);

            Throw404IfModelIsNotFound([mODEL]);
            
            TryUpdateModel([mODEL]);

            if (ModelState.IsValid)
            {
                if(OperationIsASuccess(_[mODEL]Service.Update([mODEL], User.Identity.Name)));
                return RedirectToAction("Index");
            }

            SetupEditViewBagItems([mODEL]);

            return View([mODEL]);
        }
        
        /// <summary>
        /// Post Delete Method to remove the <see cref="I[MODEL]" />
        /// </summary>
        /// <param name="id">
        ///     Id of <see cref="I[MODEL]" /> to Remove
        /// </param>
        /// <returns>
        ///     Fine - Json with True
        ///     Not Fine - Json with False
        /// </returns>
        [HttpPost]
        [MiscFieldActionFilter(MISC_FIELD_TABLE_ID)]
        public JsonResult Delete(int id)
        {
            var [mODEL] = _[mODEL]Service.Get(id);

            Throw404IfModelIsNotFound([mODEL]);

            return Json(_[mODEL]Service.Delete([mODEL], User.Identity.Name));
        }

        private void SetupEditViewBagItems(I[MODEL] instance)
        {
            ViewBag.Title = ViewBag.Title = string.Format("{0} - {1}", GetTranslation("SOA_MODEL_[MODEL_UPPER]", "[MODEL_DISPLAYNAME]"), GetTranslation("EDIT", "Edit"));
            ViewBag.DeleteConfirmation = GetTranslation("[MODEL_UPPER]_DELETE_CONFIRMATION",
                                                        "Are you sure you want to delete this [MODEL_DISPLAYNAME_SINGULAR]?");
            ViewBag.DeleteErrorMessage = GetTranslation("[MODEL_UPPER]_DELETE_ERRORMESSAGE",
                                            "Unable to remove record. This type is being used by other records.");


            ViewBag.DeleteUrl = Url.Action("Delete", new { instance.ID });
            ViewBag.DeleteSuccessUrl = Url.Action("Index");

            //If not deleteable the Delete button won't be displayed 
            @ViewBag.DeleteEnabled = _[mODEL]Service.IsDeletable(instance, User.Identity.Name).IsSuccessful;
        }
    }
}