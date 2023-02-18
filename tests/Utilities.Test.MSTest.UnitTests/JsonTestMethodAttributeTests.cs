using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

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
            forecast.Should().NotBeNull();
        }

        [TestMethod]
        public void JsonTestMethod_RandomStringAsFile_ShouldThrow()
        {
            Action act = () => _ = new JsonTestMethodAttribute($"{Guid.NewGuid()}", typeof(WeatherForecast));
            act.Should().Throw<FileNotFoundException>();
        }

        [TestMethod]
        public void JsonTestMethod()
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            Action act = () => _ = new JsonTestMethodAttribute(null!, typeof(WeatherForecast));
#else
            Action act = () => _ = new JsonTestMethodAttribute(null, typeof(WeatherForecast));
#endif
            act.Should().Throw<ArgumentNullException>();
        }

        // #if NETFRAMEWORK || NETSTANDARD2_0 || NETSTANDARD2_1
        // [JsonTestMethod(null, typeof(WeatherForecast))]
        // #else
        //         [JsonTestMethod(null!, typeof(WeatherForecast))]
        // #endif
        //         public void Foo()
        //         {

        //         }
    }
}