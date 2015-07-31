path: CodeSoa\trunk\Testing\Services\ByDesign.SOA.Testing.Services\[AREA]\MODELServiceTest.cs

using ByDesign.SOA.Business.Interfaces.[AREA];
using ByDesign.SOA.Common.Attributes;
using ByDesign.SOA.Common.DI;
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Data.Interfaces.[AREA];
using ByDesign.SOA.Testing.Services.Infastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using Moq;

namespace ByDesign.SOA.Testing.Services.[AREA]
{
    /// <summary>
    ///     The main Service Test to interact with <see cref="I[MODEL]Service" />
    /// </summary>
    [TestClass]
    [NoCodeCoverage]
    public class [MODEL]ServiceTest : FakeCRUDRepository<I[MODEL], I[MODEL]Repository>
    {
        private readonly I[MODEL]Service _[mODEL]Service;

        /// <summary>
        ///  <see cref="I[MODEL]Service" /> Constructor
        /// </summary>
        public [MODEL]ServiceTest()
            : base(((instance, id) => instance.ID == id), (instance, instance2) => instance.ID == instance2.ID)
        {
            var mockService = CRUDRepositoryMock();
            mockService.Setup(s => s.ValidateDelete(It.IsAny<I[MODEL]>(), Username)).Returns(GetSuccessfulOperationResult);
            _[mODEL]Service = new Business.Services.[AREA].[MODEL]Service(mockService.Object);
        }

        /// <summary>
        /// Test Method to test the Get method in the <see cref="I[MODEL]Service" />
        /// </summary>
        [TestMethod]
        public void [MODEL]ServiceTest_VerifyIEnumerableGetReturnsResults()
        {
            Assert.IsNotNull(_[mODEL]Service.Get());
        }

        /// <summary>
        /// Test Method to test the Insert method in the <see cref="I[MODEL]Service" />
        /// </summary>
        [TestMethod]
        public void [MODEL]ServiceTest_InsertAndGet()
        {
            var c = Container.Instance.Resolve<I[MODEL]>();
            c.ID = 10000000;
            c.Description = "UnitTestDescription";
            _[mODEL]Service.Create(c, Username);
            //TODO PW#[TICKET] Add all properties 
            I[MODEL] ci;
            ci = _[mODEL]Service.Get(10000000);
            Assert.IsNotNull(ci);
        }

        /// <summary>
        /// Test Method to test the Delete method in the <see cref="I[MODEL]Service" />
        /// </summary>
        [TestMethod]
        public void [MODEL]ServiceTest_InsertAndDelete()
        {
            var c = Container.Instance.Resolve<I[MODEL]>();
            c.ID = 10000001;
            c.Description = "UnitTestDescription2";

            Assert.IsTrue(_[mODEL]Service.Create(c, Username).IsSuccessful);
            var ci = _[mODEL]Service.Get(10000001);

            var deleteResult = _[mODEL]Service.Delete(ci, Username).IsSuccessful;
            Assert.IsTrue(deleteResult);
        }

        /// <summary>
        /// Test Method to test the Update method in the <see cref="I[MODEL]Service" />
        /// </summary>
        [TestMethod]
        public void [MODEL]ServiceTest_InsertAndUpdate()
        {
            var c = Container.Instance.Resolve<I[MODEL]>();
            c.ID = 10000002;
            c.Description = "UnitTestDescription3";
            Assert.IsTrue(_[mODEL]Service.Create(c, Username).IsSuccessful);

            var d = Container.Instance.Resolve<I[MODEL]>();
            d.ID = 10000003;
            d.Description = "UnitTestDescription3 Updated";
            Assert.IsTrue(_[mODEL]Service.Update(d, Username).IsSuccessful);

            var updated = _[mODEL]Service.Get(10000003);
            if (updated != null)
            {
                Assert.IsTrue(updated.Description == "UnitTestDescription3 Updated");
            }

        }

        /// <summary>
        /// Test method to test Exception NULL Object in the Insert method of <see cref="I[MODEL]Service" />
        /// </summary>
        [TestMethod]
        [ExpectedException((typeof(ArgumentNullException)))]
        public void [MODEL]ServiceTest_InsertThrowsExceptionOnNullInput()
        {
            _[mODEL]Service.Create(null, Username);
        }

        /// <summary>
        /// Test method to test Exception NULL user name in the Insert method of <see cref="I[MODEL]Service" />
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void [MODEL]ServiceTest_InsertThrowsExceptionOnNullUsername()
        {
            var c = Container.Instance.Resolve<I[MODEL]>();
            c.Description = "UnitTestDescription User null";
            _[mODEL]Service.Create(c, null);
        }

        /// <summary>
        /// Test method to test Exception NULL user name in the Update null method of <see cref="I[MODEL]Service" />
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void [MODEL]ServiceTest_UpdateThrowsExceptionOnNullUsername()
        {
            var c = Container.Instance.Resolve<I[MODEL]>();
            c.Description = "UnitTestDescription Update User null";
            _[mODEL]Service.Update(c, null);
        }

        /// <summary>
        /// Test method to test the Exception NULL object in the Update method of <see cref="I[MODEL]Service" />
        /// </summary>
        [TestMethod]
        [ExpectedException((typeof(ArgumentNullException)))]
        public void [MODEL]ServiceTest_UpdateThrowsExceptionOnNullInput()
        {
            _[mODEL]Service.Update(null, Username);
        }

        /// <summary>
        /// Test method to test the Exception NULL object in the delete method of <see cref="I[MODEL]Service" />
        /// </summary>
        [TestMethod]
        [ExpectedException((typeof(ArgumentNullException)))]
        public void [MODEL]ServiceTest_DeleteThrowsExceptionOnNullInput()
        {
            _[mODEL]Service.Delete(null, Username);
        }
    }
}