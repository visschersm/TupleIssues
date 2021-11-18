using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matr.Utilities.Test;
using FluentAssertions;

namespace Matr.Utilities.Test.UnitTests
{
    [TestClass]
    public class FluentRegistrationTests : TestBase
    {
        [TestMethod]
        public void Test()
        {
            factory.RegisterOrReplaceService(new FluentRegistrationTestService());

            // var result = factory.GetRegisteredService();

            // result.Should().HaveCount(1);
        }

        private class FluentRegistrationTestService {}
        private class FluentRegistrationAnotherTestService {}
    }
}