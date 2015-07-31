using ByDesign.SOA.Business.Interfaces.[AREA];
using ByDesign.SOA.Common.Attributes;
using ByDesign.SOA.Common.IModels.[AREA];
using ByDesign.SOA.Data.Interfaces.[AREA];
using ByDesign.SOA.Testing.Services.Infastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}