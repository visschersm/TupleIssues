using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

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
            factory.RegisterOrReplace(new TestServiceWithoutDependencies())
                .RegisterOrReplace(Substitute.For<ITestDependencyInterface>())
                .RegisterOrReplace(Substitute.For<IOtherDependencyInterface>());

            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            Assert.IsNotEmpty(result);
        }
    }

    public class TestServiceWithoutDependencies { }

    public interface ITestDependencyInterface { }

    public interface IOtherDependencyInterface { }

    public class TestServiceWithDependencies
    {
        public TestServiceWithDependencies(ITestDependencyInterface _)
        {
        }
    }

    public class TestServiceWithOtherDependencies
    {
        public TestServiceWithOtherDependencies(IOtherDependencyInterface _)
        {
        }
    }
}