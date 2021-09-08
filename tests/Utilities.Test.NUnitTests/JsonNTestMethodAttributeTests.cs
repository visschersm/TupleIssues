using Matr.Utilities.NTest.Attributes;
using NUnit.Framework;
using System;
using FluentAssertions;

namespace Matr.Utilities.Test.NUnitTests
{
    public class Foo
    {
        [FooJsonTest("data.json", typeof(WeatherForecast))]
        public void Test(WeatherForecast forecast)
        {
            forecast.Should().NotBeNull();
        }

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
    }
}