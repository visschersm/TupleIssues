using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;


#if NET7_0_OR_GREATER
using System.Collections.Generic;
#endif

namespace Matr.Utilities.Test.MSTest.Attributes
{
#if NET7_0_OR_GREATER
    [AttributeUsage(System.AttributeTargets.Method)]
    public partial class JsonTestMethodAttribute<TType> : TestMethodAttribute
    {
        private readonly string _filepath;

        public JsonTestMethodAttribute(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
                throw new ArgumentNullException(nameof(filepath));

            _filepath = File.Exists(filepath) ? filepath : throw new FileNotFoundException(filepath);
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            using var r = new StreamReader(_filepath);
            var json = r.ReadToEnd();

            var data = JsonSerializer.Deserialize<IEnumerable<TType>>(json)
                ?? throw new JsonException();
                    
            return data
                .Where(x => x is not null)
                .Select(x => testMethod.Invoke(new object[] { x! }))
                .ToArray();
        }
    }
#endif

    [AttributeUsage(AttributeTargets.Method)]
    public class JsonTestMethodAttribute : TestMethodAttribute
    {
        private readonly string _filepath;
        private readonly Type _dataType;
        
        public JsonTestMethodAttribute(string filepath, Type dataType)
        {
            if (string.IsNullOrWhiteSpace(filepath))
                throw new ArgumentNullException(nameof(filepath));

            _filepath = File.Exists(filepath) ? filepath : throw new FileNotFoundException(filepath);
            _dataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            using (var r = new StreamReader(_filepath))
            {
                var json = r.ReadToEnd();

                var dataArray = JsonSerializer.Deserialize(json, _dataType.MakeArrayType());
#if NETSTANDARD2_0
                return (dataArray as IEnumerable).Cast<object>()
#else
                return (dataArray as IEnumerable)!.Cast<object>()
#endif
                    .Select(x => testMethod.Invoke(new[] { x }))
                    .ToArray();
            }
        }
    }
}