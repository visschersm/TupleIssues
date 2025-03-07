using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Matr.Utilities.Test.UnitTests
{
    [TestClass]
    public class GenericFactoryTests
    {
        [TestMethod]
        public void FluentRegistration_MultiRegistered_NotBeEmpty()
        {
            // Arrange
            var factory = new GenericFactory();

            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            Assert.IsNotEmpty(result);
        }
    }
}