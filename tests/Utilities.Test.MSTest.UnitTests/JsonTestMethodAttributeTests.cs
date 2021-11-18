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
#if NETFRAMEWORK || NETSTANDARD2_0 || NETSTANDARD2_1
            public string Summary { get; set; }
#else
            public string? Summary { get; set; }
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
    }
}