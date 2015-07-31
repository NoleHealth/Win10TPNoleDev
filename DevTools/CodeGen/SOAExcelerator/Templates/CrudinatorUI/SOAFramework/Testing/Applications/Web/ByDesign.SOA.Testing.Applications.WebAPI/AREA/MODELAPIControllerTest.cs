using ByDesign.SOA.Applications.WebAPI.Areas.[AREA].Controllers;
using ByDesign.SOA.Business.Interfaces.[AREA];
using ByDesign.SOA.Business.Services.[AREA];
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Common.Models.[AREA];
using ByDesign.SOA.Data.Interfaces.[AREA];
using ByDesign.SOA.Data.Repositories.[AREA];
using ByDesign.SOA.Testing.Applications.WebAPI.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace ByDesign.SOA.Testing.Applications.WebAPI.[AREA]
{
    /// <summary>
    ///     The main Service Test to interact with <see cref="[MODEL]Controller" />
    /// </summary>
    [TestClass]
    public class [MODEL]APIControllerTest
    {
        private [MODEL]Controller _controller;
        private I[MODEL] _obj[MODEL];
        private List<I[MODEL]> _obj[MODEL]List;

        /// <summary>
        /// Initializer of Test Class
        /// </summary>
        [TestInitialize]
        public void TextFixture()
        {
            I[MODEL]Repository obj[MODEL]Repository = new [MODEL]Repository();
            I[MODEL]Service objAutoShipRuleService = new [MODEL]Service(obj[MODEL]Repository);
            _obj[MODEL]List = new List<I[MODEL]>();
            _controller = new [MODEL]Controller(objAutoShipRuleService);

            _obj[MODEL] = new [MODEL]
            {
                //TODO PW#[TICKET] Add each property
                ID = 1,
                Description = "UnitTestDescription",
                DateCreated = DateTime.Now,
                CreatedBy = "Rene Menjivar (ByDesign)"
            };

            _obj[MODEL]List.Add(_obj[MODEL]);
        }

        /// <summary>
        ///      Procedure to Setup controller Test Class
        /// </summary>
        /// <param name="obj[MODEL]Service">
        ///     <see cref="I[MODEL]Service" /> to use for test in the class
        /// </param>
        /// <returns>
        ///     <see cref="[MODEL]Controller" /> to test in the class
        /// </returns>
        protected [MODEL]Controller SetupController(I[MODEL]Service obj[MODEL]Service)
        {
            var controller = new [MODEL]Controller(obj[MODEL]Service);

            var objcon = new GetController<[MODEL]Controller>(controller);
            controller = objcon.CreateController();

            return controller;
        }

        /// <summary>
        ///     Testing Get method with MOCK object so that it will test only Controller method ....... not the full cycle which include Service, Repository, database connection etc.
        /// </summary>
        [TestMethod]
        public void [MODEL]ControllerTestGetMoq()
        {
            var obj[MODEL]Service = new Mock<I[MODEL]Service>();
            obj[MODEL]Service.Setup(c => c.Get()).Returns(_obj[MODEL]List);

            _controller = new [MODEL]Controller(obj[MODEL]Service.Object);
            var returnSet = _controller.Get();
            Assert.IsNotNull(returnSet, "Method did not return the MOQed set correctly");
        }

        /// <summary>
        ///     Testing GetByID method with MOCK object so that it will test only Controller method ....... not the full cycle which include Service, Repository, database connection etc.
        /// </summary>
        [TestMethod]
        public void [MODEL]ControllerTestGetByID()
        {
            var obj[MODEL]Service = new Mock<I[MODEL]Service>();
            obj[MODEL]Service.Setup(c => c.Get(It.IsAny<int>())).Returns(_obj[MODEL]);

            _controller = new [MODEL]Controller(obj[MODEL]Service.Object);
            var returnSet = _controller.Get(1);
            Assert.IsNotNull(returnSet, "Method did not return the MOQed set correctly");
        }
    }
}