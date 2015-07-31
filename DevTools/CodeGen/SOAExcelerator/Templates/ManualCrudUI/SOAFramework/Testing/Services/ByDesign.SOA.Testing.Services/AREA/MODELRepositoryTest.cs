using ByDesign.SOA.Common.Attributes;
using ByDesign.SOA.Data.Interfaces.[AREA];
using ByDesign.SOA.Data.Repositories.[AREA];
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Linq;

namespace ByDesign.SOA.Testing.Services.[AREA]
{
    /// <summary>
    ///     The main Service Test to interact with <see cref="I[MODEL]Repository" />
    /// </summary>
    [TestClass]
    [NoCodeCoverage]
    [DeploymentItem("connectionStrings.config")]
    [DeploymentItem("appSettings.config")]
    public class [MODEL]RepositoryTest
    {
        private I[MODEL]Repository _[mODEL]Repository;

        /// <summary>
        ///     Setup the repository.
        /// </summary>
        [TestInitialize]
        public void TextFixture()
        {
            _[mODEL]Repository = ByDesign.SOA.Common.DI.Container.Instance.Resolve<I[MODEL]Repository>();
        }

        /// <summary>
        ///     Verify we get results from a simple get method
        /// </summary>
        [TestMethod]
        [Description("Verifies Get (no parameters) works by returning something")]
        public void [MODEL]Repository_VerifyIEnumerableGetReturnsResults()
        {
            Assert.IsNotNull(_[mODEL]Repository.Get());
        }

        /// <summary>
        ///     Verify we get results from a simple get method
        /// </summary>
        [TestMethod]
        [Description("Verifies Get (with a parameter) works by returning a known single-source value.")]
        public void [MODEL]Repository_VerifyGetReturnsResults()
        {
            var e = _[mODEL]Repository.Get(1);
            Assert.IsNotNull(e);
        }

        /// <summary>
        ///     Test method to test the Update method in the <see cref="I[MODEL]Repository" />
        /// </summary>
        [TestMethod]
        [Description("Test updating value.")]
        public void [MODEL]Repository_Update()
        {
            using (var transactionScope = _[mODEL]Repository.Scope())
            {
                var e = _[mODEL]Repository.Get(1);
                e.Description = "UnitTestDescription Update";
                e.DateLastModified = System.DateTime.Now;
                e.LastModifiedBy = "Repository Test";
                Assert.IsTrue(_[mODEL]Repository.Update(e).IsSuccessful);

                var u = _rankPriceTypeRepository.Get(e.ID);
                Assert.IsTrue(e.Description == u.Description);

                transactionScope.Rollback();
            }
        }

        /// <summary>
        ///     Test a TEST_MAX_LENGTH_PROPERTY that is too long. [TEST_MAX_LENGTH_TYPE]([TEST_MAX_LENGTH_SIZE]) in the <see cref="I[MODEL]Repository" /> Update
        /// </summary>
        [TestMethod]
        [Description("Test a description that is too long. [TEST_MAX_LENGTH_TYPE]([TEST_MAX_LENGTH_SIZE])")]
        [ExpectedException(typeof(SqlException))]
        public void [MODEL]Repository_TestLong[TEST_MAX_LENGTH_PROPERTY]()
        {
            using (var transactionScope = _[mODEL]Repository.Scope())
            {
                var e = _[mODEL]Repository.Get(1);
                e.[TEST_MAX_LENGTH_PROPERTY] = string.Concat(Enumerable.Repeat("Long[TEST_MAX_LENGTH_PROPERTY]_", 20)) + "a";
                while(e.[TEST_MAX_LENGTH_PROPERTY].Length < [TEST_MAX_LENGTH_SIZE])
                    e.[TEST_MAX_LENGTH_PROPERTY] += string.Concat(Enumerable.Repeat("Long[TEST_MAX_LENGTH_PROPERTY]_", 20)) + "a";

                Assert.IsFalse(_[mODEL]Repository.Update(e).IsSuccessful);
                transactionScope.Rollback();
            }
        }


    }
}