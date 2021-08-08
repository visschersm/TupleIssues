using System;
using System.IO;
using FluentAssertions;
#if NET462 || NET48 || NETSTANDARD20 || NETSTANDARD21
using Newtonsoft.Json;
#else
using System.Text.Json;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Matr.Utilities.Test.Attributes.UnitTests
{
    [TestClass]
    public class JsonTestMethodAttributeTests
    {
        public class WeatherForecast
        {
            public DateTime Date { get; set; }
            public int TemperatureCelsius { get; set; }
#if NET462 || NET48 || NETSTANDARD20 || NETSTANDARD21
            public string Summary { get; set; }
#else
            public string? Summary { get; set; }
#endif
        }

        [ClassInitialize]
        public static void Init(TestContext _)
        {
            var data = new WeatherForecast[]
            {
                new WeatherForecast
                {
                    Date = DateTime.Parse("2021-07-31"),
                    TemperatureCelsius = 25,
                    Summary = "Hot"
                },
                new WeatherForecast
                {
                    Date = DateTime.Parse("2021-08-07"),
                    TemperatureCelsius = 25,
                    Summary = "Hot"
                },
            };

#if NET462 || NET48 || NETSTANDARD20 || NETSTANDARD21
            var json = JsonConvert.SerializeObject(data);
#else
            var json = JsonSerializer.Serialize(data);
#endif

            File.WriteAllText("data.json", json);
        }

        [JsonTestMethod("data.json", typeof(WeatherForecast))]
        public void Test(WeatherForecast forecast)
        {
            forecast.Should().NotBeNull();
        }

        [TestMethod]
        public void JsonTestMethod_RandomStringAsFile_ShouldThrow()
        {
            Action act = () => new JsonTestMethodAttribute($"{Guid.NewGuid()}", typeof(WeatherForecast));
            act.Should().Throw<FileNotFoundException>();
        }
    }
}