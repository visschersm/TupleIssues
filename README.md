# MatrTech.Utilities
Some utilities to make it easier to test in C#.
# MatrTech.Utilities.Test
This package provides some utilities to make it easier to test in C#. For starters it provides a lightweight dependency factory to manage all the tests dependencies. Making it easily and safe to register and replace dependencies within each test. Which provides proper state to test against.

## Installing
The package is easily installed using nuget.
```
Install-Package MatrTech.Utilities.Test
```

## Using the package
```csharp
using namespace Matr.Test;
```

That's it, now you can use the package.

### MSTest
```csharp
[TestClass]
public class TestClass : TestBase
{
    ...
}
```

### NUnit
```csharp
public class TestClass : TestBase
{
    ...
}
```

### XUnit
```Not yet implemented.```

## Package in action
### Registering dependencies
#### MSTest
```csharp
[TestInitialize]
public void TestInitialize()
{
    factory.RegisterOrReplace(new Mock<ITestDependency>().Object);
}
```
#### NUnit
```csharp
[SetUp]
public void Init()
{
    factory.RegisterOrReplace(new Mock<ITestDependency>().Object);
}
```

#### XUnit
```Not yet implemented.```

### Using in tests
#### MSTest
```csharp
[TestMethod]
public void TestMethod()
{
    // Setup test specifc dependencies.
    var mockedDependency = new Mock<ITestDependency>();
    
    // Mock functionality specific to the test.
    mockedDependency.Setup(...).Returns(...);

    // Register test specifc dependencies.
    factory.RegisterOrReplace(mockedDependency.Object);
    
    // Create your service with the Create method.
    // This will resolve all dependencies.
    var serviceToTest = factory.Create<ServiceToTest>();
    
    var result = serviceToTest.MethodToTest();

    Assert.Equal(result, expectedValue);
}
```

#### NUnit
Same goes for NUnit
```csharp
[Test]
public void TestMethod()
{
    // Setup test specifc dependencies.
    var mockedDependency = new Mock<ITestDependency>();
    
    // Mock functionality specific to the test.
    mockedDependency.Setup(...).Returns(...);

    // Register test specifc dependencies.
    factory.RegisterOrReplace(mockedDependency.Object);
    
    // Create your service with the Create method.
    // This will resolve all dependencies.
    var serviceToTest = factory.Create<ServiceToTest>();
    
    var result = serviceToTest.MethodToTest();

    Assert.Equal(result, expectedValue);
}
```

#### XUnit
```Not yet implemented.```

## Coming soon
- XUnit implementation
- JsonTestSource