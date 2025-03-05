using NUnit.Framework;
using System;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Matr.Utilities.Test.Attributes;

namespace Matr.Utilities.Test.NUnitTests
{
    public class JsonTestMethodAttributeTests
    {
#if NET7_0_OR_GREATER
        [JsonTestMethod<WeatherForecast>("data.json")]
        public void JsonTestMethod_ValidJsonFile_ShouldNotBeNull(WeatherForecast forecast)
        {
            Assert.NotNull(forecast);
        }
#endif

        [JsonTestMethod("data.json", typeof(WeatherForecast))]
        public void GenericJsonTestMethod_ValidJsonFile_ShouldNotBeNull(WeatherForecast forecast)
        {
            Assert.NotNull(forecast);
        }

        [Test]
        public void JsonTestMethod_ValidJsonFile_ShouldHaveTwoCases()
        {
            var type = this.GetType();
            var methodInfo = type.GetMethod(nameof(this.JsonTestMethod_ValidJsonFile_ShouldHaveTwoCases));
            var result = new JsonTestMethodAttribute("data.json", typeof(WeatherForecast))
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                .BuildFrom(new NUnit.Framework.Internal.MethodWrapper(type, methodInfo!), null);
#else
                .BuildFrom(new NUnit.Framework.Internal.MethodWrapper(type, methodInfo), null);
#endif

            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void JsonTestMethod_FilePathNull_ArgumentNullException()
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            Func<JsonTestMethodAttribute> func = () => new JsonTestMethodAttribute(null!, typeof(WeatherForecast));
#else
            Func<JsonTestMethodAttribute> func = () => new JsonTestMethodAttribute(null, typeof(WeatherForecast));
#endif
            Assert.That(func, Throws.Exception.TypeOf<ArgumentNullException>());
        }

        // [Test]
        // public void JsonTestMethod_NonsesForJsonFile_InvalidExtensionException()
        // {
        //     var type = this.GetType();
        //     var methodInfo = type.GetMethod(nameof(this.JsonTestMethod_NonsesForJsonFile_InvalidExtensionException));
        //     var result = new JsonTestMethodAttribute("some random text", typeof(WeatherForecast))
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