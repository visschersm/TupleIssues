using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace Matr.Utilities.Test.Attributes
{
#if NET7_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class JsonTestMethodAttribute<TType> : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
        private readonly string _filepath;
        
        public JsonTestMethodAttribute(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
                throw new ArgumentNullException(nameof(filepath));

            _filepath = File.Exists(filepath) ? filepath : throw new FileNotFoundException(filepath);
        }

        /// <summary>
        /// Builds any number of tests from the specified method and context.
        /// </summary>
        /// <param name="method">The IMethod for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, global::NUnit.Framework.Internal.Test? suite)
        {
            foreach (TestCaseParameters parms in GetTestCasesFor(method))
            {
                yield return _builder.BuildTestMethod(method, suite, parms);
            }
        }

        private IEnumerable<ITestCaseData> GetTestCasesFor(IMethodInfo method)
        {
            if (!File.Exists(_filepath))
            {
                return new List<ITestCaseData>
                {
                    new TestCaseParameters(new FileNotFoundException(_filepath))
                };
            }

            using var r = new StreamReader(_filepath);
            var json = r.ReadToEnd();

            var data = JsonSerializer.Deserialize<IEnumerable<TType>>(json)
                ?? throw new JsonException();
            
            return data
                .Where(x => x is not null)
                .Select(x => new TestCaseParameters(new object[] { x! }))
                .ToList();
        }
    }
#endif

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class JsonTestMethodAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
        private readonly string _filepath;
        private readonly Type _dataType;

        public JsonTestMethodAttribute(string filepath, Type dataType)
        {
            if (string.IsNullOrWhiteSpace(filepath)) throw new ArgumentNullException(nameof(filepath));

            this._filepath = filepath;
            this._dataType = dataType;
        }

        /// <summary>
        /// Builds any number of tests from the specified method and context.
        /// </summary>
        /// <param name="method">The IMethod for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
#if NETSTANDARD2_0
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, NUnit.Framework.Internal.Test suite)
#else
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, global::NUnit.Framework.Internal.Test? suite)
#endif
        {
            foreach (TestCaseParameters parms in GetTestCasesFor(method))
            {
                yield return _builder.BuildTestMethod(method, suite, parms);
            }
        }

        private IEnumerable<ITestCaseData> GetTestCasesFor(IMethodInfo method)
        {
            if (!File.Exists(_filepath))
                return new List<ITestCaseData>
                {
                    new TestCaseParameters(new FileNotFoundException(_filepath))
                };

            using (StreamReader r = new StreamReader(_filepath))
            {
                string json = r.ReadToEnd();

                var dataArray = JsonSerializer.Deserialize(json, _dataType.MakeArrayType());
#if NETSTANDARD2_0
                return (dataArray as IEnumerable).Cast<object>()
#else
                return (dataArray as IEnumerable)!.Cast<object>()
#endif
                    .Select(x => new TestCaseParameters(new object[] { x }))
                    .ToList();
            }
        }
    }
}