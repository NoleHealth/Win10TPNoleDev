using ByDesign.SOA.Common.Attributes;
using ByDesign.SOA.Data.Interfaces.[AREA];
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var e = _[mODEL]Repository.Get().FirstOrDefault();

            if (e == null)
                Assert.Inconclusive("No [MODEL] to test");
            else
                Assert.IsNotNull(e);
        }
    }
}