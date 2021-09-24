using Matr.Utilities.NTest.Attributes;
using NUnit.Framework;
using System;
using FluentAssertions;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Matr.Utilities.Test.NUnitTests
{
    public class JsonNTestMethodAttributeTests
    {
        [JsonNTestMethod("data.json", typeof(WeatherForecast))]
        public void JsonNTestMethod_ValidJsonFile_ShouldNotBeNull(WeatherForecast forecast)
        {
            forecast.Should().NotBeNull();
        }

        [Test]
        public void JsonNTestMethod_ValidJsonFile_ShouldHaveTwoCases()
        {
            var type = this.GetType();
            var methodInfo = type.GetMethod(nameof(this.JsonNTestMethod_ValidJsonFile_ShouldHaveTwoCases));
            var result = new JsonNTestMethodAttribute("data.json", typeof(WeatherForecast))
                .BuildFrom(new NUnit.Framework.Internal.MethodWrapper(type, methodInfo!), null);

            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Test]
        public void JsonNTestMethod_FilePathNull_ArgumentNullException()
        {
            Func<JsonNTestMethodAttribute> func = () => new JsonNTestMethodAttribute(null!, typeof(WeatherForecast));

            func.Should().Throw<ArgumentNullException>();
        }

        // [Test]
        // public void JsonNTestMethod_NonsesForJsonFile_InvalidExtensionException()
        // {
        //     var type = this.GetType();
        //     var methodInfo = type.GetMethod(nameof(this.JsonNTestMethod_NonsesForJsonFile_InvalidExtensionException));
        //     var result = new JsonNTestMethodAttribute("some random text", typeof(WeatherForecast))
        //         .BuildFrom(new NUnit.Framework.Internal.MethodWrapper(type, methodInfo!), null);

        //     result.Should().NotBeNull();
        //     result.Count().Should().Be(1);

        //     Assert.True(false);
        //     // TODO: Figure out how to valide this invalid state
        //     // result.First().MakeTestResult()..Count().Should().Be(1);
        //     // result.First().MakeTestResult().AssertionResults
        //     //     .First()
        //     //     .Should()
        //     //     .BeOfType<FileNotFoundException>();
        // }

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