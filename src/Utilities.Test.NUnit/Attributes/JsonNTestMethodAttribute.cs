using System;
using System.IO;
using System.Security;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Linq;

using NUnit.Compatibility;

namespace Matr.Utilities.NTest.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class JsonNTestMethodAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
        private readonly string filepath;
        private readonly Type dataType;

        public JsonNTestMethodAttribute(string filepath, Type dataType)
        {
            if (string.IsNullOrWhiteSpace(filepath)) throw new ArgumentNullException(nameof(filepath));

            this.filepath = filepath;
            this.dataType = dataType;
        }

        /// <summary>
        /// Builds any number of tests from the specified method and context.
        /// </summary>
        /// <param name="method">The IMethod for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
#if NETSTANDARD2_0
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, NUnit.Framework.Internal.Test suite)
#else
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, NUnit.Framework.Internal.Test? suite)
#endif
        {
            foreach (TestCaseParameters parms in GetTestCasesFor(method))
            {
                yield return _builder.BuildTestMethod(method, suite, parms);
            }
        }

        private IEnumerable<ITestCaseData> GetTestCasesFor(IMethodInfo method)
        {
            if (!File.Exists(filepath))
                return new List<ITestCaseData>
                {
                    new TestCaseParameters(new FileNotFoundException(filepath))
                };

            using (StreamReader r = new StreamReader(filepath))
            {
                string json = r.ReadToEnd();

                var dataArray = JsonSerializer.Deserialize(json, dataType.MakeArrayType());
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