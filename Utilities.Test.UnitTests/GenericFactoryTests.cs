using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using Moq;
using Autofac.Core;

namespace MatrTech.Utilities.Test.UnitTests
{
    [TestClass]
    public class GenericFactoryTests
    {
        [TestMethod]
        public void CreateWithoutDependencies_WithoutDependencies_ReturnTestObject()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            var result = factory.Create<TestServiceWithoutDependencies>();

            // Assert
            result.Should().BeOfType<TestServiceWithoutDependencies>();
        }

        [TestMethod]
        public void CreateWithDependencies_DependenciesNotRegistered_Exception()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            Func<TestServiceWithDependencies> func = () => factory.Create<TestServiceWithDependencies>();

            // Assert
            func.Should().Throw<DependencyResolutionException>();
        }

        [TestMethod]
        public void CreateWithDependencies_DependenciesRegistered_TestServiceWithDependenciesObject()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            Mock<ITestDependencyInterface> mock = new Mock<ITestDependencyInterface>();
            factory.RegisterOrReplaceService(mock.Object);

            // Act
            var result = factory.Create<TestServiceWithDependencies>();

            // Assert
            result.Should()
                .NotBeNull()
                .And
                .BeOfType<TestServiceWithDependencies>();
        }

        [TestMethod]
        public void CreateTwice_WithoutDependencies_DoesNotThrow()
        {
            GenericFactory factory = new GenericFactory();

            var result1 = factory.Create<TestServiceWithoutDependencies>();
            var result2 = factory.Create<TestServiceWithoutDependencies>();

            result1.Should().BeOfType<TestServiceWithoutDependencies>();
            result2.Should().BeOfType<TestServiceWithoutDependencies>();
        }

        [TestMethod]
        public void CreateTwice_WithDependencies_DoesNotThrow()
        {
            GenericFactory factory = new GenericFactory();

            Mock<ITestDependencyInterface> mock = new Mock<ITestDependencyInterface>();
            factory.RegisterOrReplaceService(mock.Object);

            var result1 = factory.Create<TestServiceWithDependencies>();
            var result2 = factory.Create<TestServiceWithDependencies>();

            result1.Should().BeOfType<TestServiceWithDependencies>();
            result2.Should().BeOfType<TestServiceWithDependencies>();
        }

        [TestMethod]
        public void RegisterOrReplaceService_ServiceNull_ArgumentNullException()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            ITestDependencyInterface? dependency = null;

            // Act
            Action func = () => factory.RegisterOrReplaceService(dependency!);

            // Assert
            func.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RegisterOrReplaceService_ValidService_DoesNotThrow()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();

            // Act
            Action func = () => factory.RegisterOrReplaceService(dependency.Object);

            // Assert
            func.Should().NotThrow();
        }

        [TestMethod]
        public void EmptyDependencies_DependenciesRegistered_ShouldNotBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();

            factory.RegisterOrReplaceService(dependency.Object);

            // Act
            factory.EmptyDependencies();
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void EmptyDependencies_NoDependenciesRegistered_ShouldNotBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            factory.EmptyDependencies();
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsRegistered_NotRegisterd_ShouldBeFalse()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsRegistered_IsRegistered_ShouldBeTrue()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();
            factory.RegisterOrReplaceService(dependency.Object);

            // Act
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void RemoveService_NotRegistered_ShouldNotBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            factory.RemoveService<ITestDependencyInterface>();
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void RemoveService_DependencyRegistered_ShouldNotBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();
            factory.RegisterOrReplaceService<ITestDependencyInterface>(dependency.Object);

            // Act
            factory.RemoveService<ITestDependencyInterface>();
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void RemoveService_ReRegister_ShouldBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();

            // Act
            Console.WriteLine("Register1");
            factory.RegisterOrReplaceService(dependency.Object);
            var result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeTrue();

            Console.WriteLine("Remove");
            factory.RemoveService<ITestDependencyInterface>();
            result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeFalse();

            Console.WriteLine("Register2");
            factory.RegisterOrReplaceService(dependency.Object);
            result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeTrue();
        }
        [TestMethod]
        public void CreateAfterRemove_ShouldNotThrow()
        {
            // Arrange
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();
            GenericFactory factory = new GenericFactory();

            Func<TestServiceWithDependencies> func = () =>
            {
                factory.RemoveService<ITestDependencyInterface>();
                factory.RegisterOrReplaceService(dependency.Object);
                return factory.Create<TestServiceWithDependencies>();
            };

            // Act & Assert
            func.Should().NotThrow();
        }

        [TestMethod]
        public void CreateAfterRemove_MissingDependency_ShouldThrow()
        {
            // Arrange
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();

            GenericFactory factory = new GenericFactory();
            factory.RegisterOrReplaceService(dependency.Object);

            _ = factory.Create<TestServiceWithDependencies>();

            Func<TestServiceWithDependencies> func = () =>
            {
                factory.RemoveService<ITestDependencyInterface>();
                return factory.Create<TestServiceWithDependencies>();
            };

            // Act & Assert
            func.Should().Throw<DependencyResolutionException>();

            factory.RegisterOrReplaceService(dependency.Object);
            _ = factory.Create<TestServiceWithDependencies>();
        }

        [TestMethod]
        public void RemoveAfterCreate_ShouldNotThrow()
        {
            // Arrange
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();
            GenericFactory factory = new GenericFactory();

            Func<TestServiceWithDependencies> func = () =>
            {
                factory.RegisterOrReplaceService(dependency.Object);
                var testService = factory.Create<TestServiceWithDependencies>();
                factory.RemoveService<ITestDependencyInterface>();

                return testService;
            };

            // Act & Assert
            func.Should().NotThrow();
        }
    }

    public class TestServiceWithoutDependencies { }
    public interface ITestDependencyInterface { }
    public interface IOtherDependencyInterface { }

    public class TestServiceWithDependencies
    {
        public TestServiceWithDependencies(ITestDependencyInterface _) { }
    }

    public class TestServiceWithOtherDependencies
    {
        public TestServiceWithOtherDependencies(IOtherDependencyInterface _) { }
    }
}