using Autofac.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NSubstitute;

namespace Matr.Utilities.Test.UnitTests
{
    [TestClass]
    public class GenericFactoryTests
    {
        //         [TestMethod]
        //         public void CreateWithoutDependencies_WithoutDependencies_ReturnTestObject()
        //         {
        //             // Arrange
        //             GenericFactory factory = new GenericFactory();

        //             // Act
        //             var result = factory.Create<TestServiceWithoutDependencies>();

        //             // Assert
        //             Assert.IsInstanceOfType<TestServiceWithoutDependencies>(result);
        //         }

        //         [TestMethod]
        //         public void CreateWithDependencies_DependenciesNotRegistered_Exception()
        //         {
        //             // Arrange
        //             GenericFactory factory = new GenericFactory();

        //             // Act & Assert
        //             Assert.Throws<DependencyResolutionException>(() => factory.Create<TestServiceWithDependencies>());
        //         }

        //         [TestMethod]
        //         public void CreateWithDependencies_DependenciesRegistered_TestServiceWithDependenciesObject()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();

        //             var mock = Substitute.For<ITestDependencyInterface>();
        //             factory.RegisterOrReplaceService(mock);

        //             // Act
        //             var result = factory.Create<TestServiceWithDependencies>();

        //             // Assert
        //             Assert.IsNotNull(result);
        //             Assert.IsInstanceOfType<TestServiceWithDependencies>(result);
        //         }

        //         [TestMethod]
        //         public void CreateTwice_WithoutDependencies_DoesNotThrow()
        //         {
        //             GenericFactory factory = new GenericFactory();

        //             var result1 = factory.Create<TestServiceWithoutDependencies>();
        //             var result2 = factory.Create<TestServiceWithoutDependencies>();

        //             Assert.IsInstanceOfType<TestServiceWithoutDependencies>(result1);
        //             Assert.IsInstanceOfType<TestServiceWithoutDependencies>(result2);
        //         }

        //         [TestMethod]
        //         public void CreateTwice_WithDependencies_DoesNotThrow()
        //         {
        //             var factory = new GenericFactory();

        //             var mock = Substitute.For<ITestDependencyInterface>();
        //             factory.RegisterOrReplaceService(mock);

        //             var result1 = factory.Create<TestServiceWithDependencies>();
        //             var result2 = factory.Create<TestServiceWithDependencies>();

        //             Assert.IsInstanceOfType<TestServiceWithDependencies>(result1);
        //             Assert.IsInstanceOfType<TestServiceWithDependencies>(result2);
        //         }

        //         [TestMethod]
        //         public void RegisterOrReplaceService_ServiceNull_ArgumentNullException()
        //         {
        //             // Arrange
        //             GenericFactory factory = new GenericFactory();

        //             // Act
        // #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        //             ITestDependencyInterface? dependency = null;
        //             Action func = () => factory.RegisterOrReplaceService(dependency!);
        // #else
        //             ITestDependencyInterface dependency = null;
        //             Action func = () => factory.RegisterOrReplaceService(dependency);
        // #endif
        //             // Assert
        //             Assert.Throws<ArgumentNullException>(func);
        //         }

        //         [TestMethod]
        //         public void RegisterOrReplaceService_ValidService_DoesNotThrow()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();
        //             var dependency = Substitute.For<ITestDependencyInterface>();

        //             // Act & Assert
        //             try
        //             {
        //                 factory.RegisterOrReplaceService(dependency);
        //             }
        //             catch (Exception exception)
        //             {
        //                 Assert.Fail(exception.Message);
        //             }
        //         }

        //         [TestMethod]
        //         public void EmptyDependencies_DependenciesRegistered_ShouldNotBeFound()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();
        //             var dependency = Substitute.For<ITestDependencyInterface>();

        //             factory.RegisterOrReplaceService(dependency);

        //             // Act
        //             factory.EmptyDependencies();
        //             bool result = factory.IsRegistered<ITestDependencyInterface>();

        //             // Assert
        //             Assert.IsFalse(result);
        //         }

        //         [TestMethod]
        //         public void EmptyDependencies_NoDependenciesRegistered_ShouldNotBeFound()
        //         {
        //             // Arrange
        //             GenericFactory factory = new GenericFactory();

        //             // Act
        //             factory.EmptyDependencies();
        //             bool result = factory.IsRegistered<ITestDependencyInterface>();

        //             // Assert
        //             Assert.IsFalse(result);
        //         }

        //         [TestMethod]
        //         public void IsRegistered_NotRegisterd_ShouldBeFalse()
        //         {
        //             // Arrange
        //             GenericFactory factory = new GenericFactory();

        //             // Act
        //             bool result = factory.IsRegistered<ITestDependencyInterface>();

        //             // Assert
        //             Assert.IsFalse(result);
        //         }

        //         [TestMethod]
        //         public void IsRegistered_IsRegistered_ShouldBeTrue()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();
        //             var dependency = Substitute.For<ITestDependencyInterface>();
        //             factory.RegisterOrReplaceService(dependency);

        //             // Act
        //             bool result = factory.IsRegistered<ITestDependencyInterface>();

        //             // Assert
        //             Assert.IsTrue(result);
        //         }

        //         [TestMethod]
        //         public void RemoveService_NotRegistered_ShouldNotBeFound()
        //         {
        //             // Arrange
        //             GenericFactory factory = new GenericFactory();

        //             // Act
        //             factory.RemoveService<ITestDependencyInterface>();
        //             bool result = factory.IsRegistered<ITestDependencyInterface>();

        //             // Assert
        //             Assert.IsFalse(result);
        //         }

        //         [TestMethod]
        //         public void RemoveService_DependencyRegistered_ShouldNotBeFound()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();
        //             var dependency = Substitute.For<ITestDependencyInterface>();
        //             factory.RegisterOrReplaceService<ITestDependencyInterface>(dependency);

        //             // Act
        //             factory.RemoveService<ITestDependencyInterface>();
        //             bool result = factory.IsRegistered<ITestDependencyInterface>();

        //             // Assert
        //             Assert.IsFalse(result);
        //         }

        //         [TestMethod]
        //         public void RemoveService_ReRegister_ShouldBeFound()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();
        //             var dependency = Substitute.For<ITestDependencyInterface>();

        //             // Act
        //             factory.RegisterOrReplaceService(dependency);
        //             var result = factory.IsRegistered<ITestDependencyInterface>();
        //             Assert.IsTrue(result);

        //             factory.RemoveService<ITestDependencyInterface>();
        //             result = factory.IsRegistered<ITestDependencyInterface>();
        //             Assert.IsFalse(result);

        //             factory.RegisterOrReplaceService(dependency);
        //             result = factory.IsRegistered<ITestDependencyInterface>();
        //             Assert.IsTrue(result);
        //         }

        //         [TestMethod]
        //         public void RemoveService_DoubleRegistration_ShouldNotBeFound()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();
        //             var dependency = Substitute.For<ITestDependencyInterface>();
        //             var dependency2 = Substitute.For<ITestDependencyInterface>();

        //             // Act
        //             factory.RegisterOrReplaceService(dependency);
        //             factory.RegisterOrReplaceService(dependency2);

        //             var result = factory.IsRegistered<ITestDependencyInterface>();
        //             Assert.IsTrue(result);

        //             factory.RemoveService<ITestDependencyInterface>();
        //             result = factory.IsRegistered<ITestDependencyInterface>();
        //             Assert.IsFalse(result);
        //         }

        //         [TestMethod]
        //         public void CreateAfterRemove_ShouldNotThrow()
        //         {
        //             // Arrange
        //             var dependency = Substitute.For<ITestDependencyInterface>();
        //             GenericFactory factory = new GenericFactory();

        //             Func<TestServiceWithDependencies> func = () =>
        //             {
        //                 factory.RemoveService<ITestDependencyInterface>();
        //                 factory.RegisterOrReplaceService(dependency);
        //                 return factory.Create<TestServiceWithDependencies>();
        //             };

        //             // Act & Assert
        //             try
        //             {
        //                 func();
        //             }
        //             catch (Exception exception)
        //             {
        //                 Assert.Fail(exception.Message);
        //             }
        //         }

        //         [TestMethod]
        //         public void CreateAfterRemove_MissingDependency_ShouldThrow()
        //         {
        //             // Arrange
        //             var dependency = Substitute.For<ITestDependencyInterface>();

        //             GenericFactory factory = new GenericFactory();
        //             factory.RegisterOrReplaceService(dependency);

        //             _ = factory.Create<TestServiceWithDependencies>();

        //             // Act & Assert
        //             Assert.Throws<DependencyResolutionException>(() =>
        //             {
        //                 factory.RemoveService<ITestDependencyInterface>();
        //                 factory.Create<TestServiceWithDependencies>();
        //             });

        //             factory.RegisterOrReplaceService(dependency);
        //             _ = factory.Create<TestServiceWithDependencies>();
        //         }

        //         [TestMethod]
        //         public void RemoveAfterCreate_ShouldNotThrow()
        //         {
        //             // Arrange
        //             var dependency = Substitute.For<ITestDependencyInterface>();
        //             GenericFactory factory = new GenericFactory();

        //             Func<TestServiceWithDependencies> func = () =>
        //             {
        //                 factory.RegisterOrReplaceService(dependency);
        //                 var testService = factory.Create<TestServiceWithDependencies>();
        //                 factory.RemoveService<ITestDependencyInterface>();

        //                 return testService;
        //             };

        //             // Act & Assert
        //             try
        //             {
        //                 func();
        //             }
        //             catch (Exception exception)
        //             {
        //                 Assert.Fail(exception.Message);
        //             }
        //         }

        //         [TestMethod]
        //         public void GetRegisteredServices_NonRegistered_EmptyList()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();

        //             // Act
        //             var result = factory.GetRegisteredServices();

        //             // Assert
        //             Assert.IsEmpty(result);
        //         }

        //         [TestMethod]
        //         public void GetRegisteredServices_OneRegistered_ListOfOne()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();
        //             factory.RegisterOrReplaceService(new TestServiceWithoutDependencies());

        //             // Act
        //             var result = factory.GetRegisteredServices();

        //             // Assert
        //             Assert.HasCount(1, result);
        //         }

        //         [TestMethod]
        //         public void GetRegisteredServices_MultiRegistered_NotBeEmpty()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();
        //             factory.RegisterOrReplaceService(new TestServiceWithoutDependencies());
        //             factory.RegisterOrReplaceService(Substitute.For<ITestDependencyInterface>());
        //             factory.RegisterOrReplaceService(Substitute.For<IOtherDependencyInterface>());

        //             // Act
        //             var result = factory.GetRegisteredServices();

        //             // Assert
        //             Assert.IsNotEmpty(result);
        //         }

        //         [TestMethod]
        //         public void FluentRegistration_OneRegistered_NotBeEmpty()
        //         {
        //             // Arrange
        //             var factory = new GenericFactory();
        //             factory.RegisterOrReplaceService(new TestServiceWithoutDependencies());

        //             // Act
        //             var result = factory.GetRegisteredServices();

        //             // Assert
        //             Assert.IsNotEmpty(result);
        //         }

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