using Autofac.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NSubstitute;

namespace Matr.Utilities.Test.UnitTests
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
            var factory = new GenericFactory();

            var mock = Substitute.For<ITestDependencyInterface>();
            factory.RegisterOrReplaceService(mock);

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
            var factory = new GenericFactory();

            var mock = Substitute.For<ITestDependencyInterface>();
            factory.RegisterOrReplaceService(mock);

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

            // Act
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            ITestDependencyInterface? dependency = null;
            Action func = () => factory.RegisterOrReplaceService(dependency!);
#else
            ITestDependencyInterface dependency = null;
            Action func = () => factory.RegisterOrReplaceService(dependency);
#endif
            // Assert
            func.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RegisterOrReplaceService_ValidService_DoesNotThrow()
        {
            // Arrange
            var factory = new GenericFactory();
            var dependency = Substitute.For<ITestDependencyInterface>();

            // Act
            Action func = () => factory.RegisterOrReplaceService(dependency);

            // Assert
            func.Should().NotThrow();
        }

        [TestMethod]
        public void EmptyDependencies_DependenciesRegistered_ShouldNotBeFound()
        {
            // Arrange
            var factory = new GenericFactory();
            var dependency = Substitute.For<ITestDependencyInterface>();

            factory.RegisterOrReplaceService(dependency);

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
            var factory = new GenericFactory();
            var dependency = Substitute.For<ITestDependencyInterface>();
            factory.RegisterOrReplaceService(dependency);

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
            var factory = new GenericFactory();
            var dependency = Substitute.For<ITestDependencyInterface>();
            factory.RegisterOrReplaceService<ITestDependencyInterface>(dependency);

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
            var factory = new GenericFactory();
            var dependency = Substitute.For<ITestDependencyInterface>();

            // Act
            factory.RegisterOrReplaceService(dependency);
            var result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeTrue();

            factory.RemoveService<ITestDependencyInterface>();
            result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeFalse();

            factory.RegisterOrReplaceService(dependency);
            result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeTrue();
        }

        [TestMethod]
        public void RemoveService_DoubleRegistration_ShouldNotBeFound()
        {
            // Arrange
            var factory = new GenericFactory();
            var dependency = Substitute.For<ITestDependencyInterface>();
            var dependency2 = Substitute.For<ITestDependencyInterface>();

            // Act
            factory.RegisterOrReplaceService(dependency);
            factory.RegisterOrReplaceService(dependency2);

            var result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeTrue();

            factory.RemoveService<ITestDependencyInterface>();
            result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeFalse();
        }

        [TestMethod]
        public void CreateAfterRemove_ShouldNotThrow()
        {
            // Arrange
            var dependency = Substitute.For<ITestDependencyInterface>();
            GenericFactory factory = new GenericFactory();

            Func<TestServiceWithDependencies> func = () =>
            {
                factory.RemoveService<ITestDependencyInterface>();
                factory.RegisterOrReplaceService(dependency);
                return factory.Create<TestServiceWithDependencies>();
            };

            // Act & Assert
            func.Should().NotThrow();
        }

        [TestMethod]
        public void CreateAfterRemove_MissingDependency_ShouldThrow()
        {
            // Arrange
            var dependency = Substitute.For<ITestDependencyInterface>();

            GenericFactory factory = new GenericFactory();
            factory.RegisterOrReplaceService(dependency);

            _ = factory.Create<TestServiceWithDependencies>();

            Func<TestServiceWithDependencies> func = () =>
            {
                factory.RemoveService<ITestDependencyInterface>();
                return factory.Create<TestServiceWithDependencies>();
            };

            // Act & Assert
            func.Should().Throw<DependencyResolutionException>();

            factory.RegisterOrReplaceService(dependency);
            _ = factory.Create<TestServiceWithDependencies>();
        }

        [TestMethod]
        public void RemoveAfterCreate_ShouldNotThrow()
        {
            // Arrange
            var dependency = Substitute.For<ITestDependencyInterface>();
            GenericFactory factory = new GenericFactory();

            Func<TestServiceWithDependencies> func = () =>
            {
                factory.RegisterOrReplaceService(dependency);
                var testService = factory.Create<TestServiceWithDependencies>();
                factory.RemoveService<ITestDependencyInterface>();

                return testService;
            };

            // Act & Assert
            func.Should().NotThrow();
        }

        [TestMethod]
        public void GetRegisteredServices_NonRegistered_EmptyList()
        {
            // Arrange
            var factory = new GenericFactory();

            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetRegisteredServices_OneRegistered_ListOfOne()
        {
            // Arrange
            var factory = new GenericFactory();
            factory.RegisterOrReplaceService(new TestServiceWithoutDependencies());

            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            result.Should().HaveCount(1);
        }

        [TestMethod]
        public void GetRegisteredServices_MultiRegistered_NotBeEmpty()
        {
            // Arrange
            var factory = new GenericFactory();
            factory.RegisterOrReplaceService(new TestServiceWithoutDependencies());
            factory.RegisterOrReplaceService(Substitute.For<ITestDependencyInterface>());
            factory.RegisterOrReplaceService(Substitute.For<IOtherDependencyInterface>());

            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            result.Should().NotBeEmpty()
                .And.NotHaveCount(1);
        }

        [TestMethod]
        public void FluentRegistration_OneRegistered_NotBeEmpty()
        {
            // Arrange
            var factory = new GenericFactory();
            factory.RegisterOrReplaceService(new TestServiceWithoutDependencies());

            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            result.Should().NotBeEmpty();
        }

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
            result.Should().NotBeEmpty()
                .And.NotHaveCount(1);
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