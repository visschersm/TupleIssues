using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Matr.Utilities.Test.MSTest.Attributes;

namespace Matr.Utilities.Test.Attributes.UnitTests
{
    [TestClass]
    public class JsonTestMethodAttributeTests
    {
        public class WeatherForecast
        {
            public DateTime Date { get; set; }
            public int TemperatureCelsius { get; set; }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            public string? Summary { get; set; }
#else
            public string Summary { get; set; }
#endif
        }

        [JsonTestMethod("data.json", typeof(WeatherForecast))]
        public void Test(WeatherForecast forecast)
        {
            Assert.IsNotNull(forecast);
        }

#if NET7_0_OR_GREATER
        [JsonTestMethod<WeatherForecast>("data.json")]
        public void GenericTest_ValidJsonFile_ReturnsObject(WeatherForecast forecast)
        {
            Assert.IsNotNull(forecast);
        }
#endif

        [TestMethod]
        public void JsonTestMethod_RandomStringAsFile_ShouldThrow()
        {
            Action act = () => _ = new JsonTestMethodAttribute($"{Guid.NewGuid()}", typeof(WeatherForecast));
            Assert.Throws<FileNotFoundException>(act);
        }

        [TestMethod]
        public void JsonTestMethodAttribute_FileNull_ArgumentNullException()
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            Action act = () => _ = new JsonTestMethodAttribute(null!, typeof(WeatherForecast));
#else
            Action act = () => _ = new JsonTestMethodAttribute(null, typeof(WeatherForecast));
#endif
            Assert.Throws<ArgumentNullException>(act);
        }
    }
}