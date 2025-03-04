using Matr.Utilities.NTest.Attributes;
using NUnit.Framework;
using System;
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
            Assert.NotNull(forecast);
        }

        [Test]
        public void JsonNTestMethod_ValidJsonFile_ShouldHaveTwoCases()
        {
            var type = this.GetType();
            var methodInfo = type.GetMethod(nameof(this.JsonNTestMethod_ValidJsonFile_ShouldHaveTwoCases));
            var result = new JsonNTestMethodAttribute("data.json", typeof(WeatherForecast))
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                .BuildFrom(new NUnit.Framework.Internal.MethodWrapper(type, methodInfo!), null);
#else
                .BuildFrom(new NUnit.Framework.Internal.MethodWrapper(type, methodInfo), null);
#endif

            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void JsonNTestMethod_FilePathNull_ArgumentNullException()
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            Func<JsonNTestMethodAttribute> func = () => new JsonNTestMethodAttribute(null!, typeof(WeatherForecast));
#else
            Func<JsonNTestMethodAttribute> func = () => new JsonNTestMethodAttribute(null, typeof(WeatherForecast));
#endif
            Assert.That(func, Throws.Exception.TypeOf<ArgumentNullException>());
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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            public string? Summary { get; set; }
#else
            public string Summary { get; set; }
#endif
        }
    }
}