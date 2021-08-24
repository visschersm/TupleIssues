using NUnit.Framework;
using Moq;

#if NET5_0 || NETCOREAPP3_1 || NETFRAMEWORK
using System;
#endif

namespace Matr.Utilities.Test.UnitTests
{
    public class NUnitTestExamples : TestBase
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleExample_WithDependency_ResolvesMockedDependency()
        {
            var random = new Random();
            var returnValue = random.Next(1, int.MaxValue);

            var mockedDependency = new Mock<ITestDependency>();
            mockedDependency.Setup(x => x.Add(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(returnValue);
            
            factory.RegisterOrReplaceService(mockedDependency.Object);
            var testClass = factory.Create<TestClass>();

            var result = testClass.DoAddition(1, 1);

            Assert.AreEqual(returnValue, result);
        }

        public class TestClass
        {
            private readonly ITestDependency dependency;

            public TestClass(ITestDependency testDependency)
            {
                this.dependency = testDependency;
            }

           public int DoAddition(int x, int y)
           {
                return dependency.Add(x, y);
           }
        }

        public interface ITestDependency
        {
            int Add(int x, int y);
        }
    }
}