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

using NUnit.Compatibility;

namespace Matr.Utilities.NTest.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FooJsonTestAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        private readonly string filepath;
        private readonly Type dataType;
        public FooJsonTestAttribute(string filepath, Type dataType)
        {
            _ = File.Exists(filepath) ? filepath : throw new FileNotFoundException(filepath);
            _ = dataType ?? throw new ArgumentNullException(nameof(dataType));

            this.filepath = filepath;
            this.dataType = dataType;
        }

        #region Properties
        /// <summary>
        /// A set of parameters passed to the method, works only if the Source Name is a method.
        /// If the source name is a field or property has no effect.
        /// </summary>
        public object?[]? MethodParams { get; }
        /// <summary>
        /// The name of a the method, property or field to be used as a source
        /// </summary>
        public string? SourceName { get; }

        /// <summary>
        /// A Type to be used as a source
        /// </summary>
        public Type? SourceType { get; }

        /// <summary>
        /// Gets or sets the category associated with every fixture created from
        /// this attribute. May be a single category or a comma-separated list.
        /// </summary>
        public string? Category { get; set; }

        #endregion

        #region ITestBuilder Members

        /// <summary>
        /// Builds any number of tests from the specified method and context.
        /// </summary>
        /// <param name="method">The IMethod for which tests are to be constructed.</param>
        /// <param name="suite">The suite to which the tests will be added.</param>
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, NUnit.Framework.Internal.Test? suite)
        {
            int count = 0;

            foreach (TestCaseParameters parms in GetTestCasesFor(method))
            {
                count++;
                yield return _builder.BuildTestMethod(method, suite, parms);
            }

            // If count > 0, error messages will be shown for each case
            // but if it's 0, we need to add an extra "test" to show the message.
            if (count == 0 && method.GetParameters().Length == 0)
            {
                var parms = new TestCaseParameters();
                parms.RunState = RunState.NotRunnable;
                parms.Properties.Set(PropertyNames.SkipReason, "TestCaseSourceAttribute may not be used on a method without parameters");

                yield return _builder.BuildTestMethod(method, suite, parms);
            }
        }

        #endregion

        #region Helper Methods

        [SecuritySafeCritical]
        private IEnumerable<ITestCaseData> GetTestCasesFor(IMethodInfo method)
        {
            List<ITestCaseData> data = new List<ITestCaseData>();

            try
            {
                IEnumerable? source = GetTestCaseSource(method);

                if (source != null)
                {
                    foreach (object? item in source)
                    {
                        // First handle two easy cases:
                        // 1. Source is null. This is really an error but if we
                        //    throw an exception we simply get an invalid fixture
                        //    without good info as to what caused it. Passing a
                        //    single null argument will cause an error to be
                        //    reported at the test level, in most cases.
                        // 2. User provided an ITestCaseData and we just use it.
                        ITestCaseData? parms = item == null
                            ? new TestCaseParameters(new object?[] { null })
                            : item as ITestCaseData;

                        if (parms == null)
                        {
                            object?[]? args = null;

                            // 3. An array was passed, it may be an object[]
                            //    or possibly some other kind of array, which
                            //    TestCaseSource can accept.
                            var array = item as Array;
                            if (array != null)
                            {
                                // If array has the same number of elements as parameters
                                // and it does not fit exactly into single existing parameter
                                // we believe that this array contains arguments, not is a bare
                                // argument itself.
                                var parameters = method.GetParameters();
                                var argsNeeded = parameters.Length;
                                if (argsNeeded > 0 && argsNeeded == array.Length && parameters[0].ParameterType != array.GetType())
                                {
                                    args = new object?[array.Length];
                                    for (var i = 0; i < array.Length; i++)
                                        args[i] = array.GetValue(i);
                                }
                            }

                            if (args == null)
                            {
                                args = new object?[] { item };
                            }

                            parms = new TestCaseParameters(args);
                        }

                        if (this.Category != null)
                            foreach (string cat in this.Category.Split(new char[] { ',' }))
                                parms.Properties.Add(PropertyNames.Category, cat);

                        data.Add(parms);
                    }
                }
                else
                {
                    data.Clear();
                    data.Add(new TestCaseParameters(new Exception("The test case source could not be found.")));
                }
            }
            catch (Exception ex)
            {
                data.Clear();
                data.Add(new TestCaseParameters(ex));
            }

            return data;
        }

        private IEnumerable? GetTestCaseSource(IMethodInfo method)
        {
            Type sourceType = SourceType ?? method.TypeInfo.Type;

            // Handle Type implementing IEnumerable separately
            if (SourceName == null)
                return Reflect.Construct(sourceType, null) as IEnumerable;

            MemberInfo[] members = sourceType.GetMember(SourceName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (members.Length == 1)
            {
                MemberInfo member = members[0];

                var field = member as FieldInfo;
                if (field != null)
                    return field.IsStatic
                        ? (MethodParams == null
                            ? (IEnumerable?)field.GetValue(null)
                            : ReturnErrorAsParameter(ParamGivenToField))
                        : ReturnErrorAsParameter(SourceMustBeStatic);

                var property = member as PropertyInfo;
                if (property != null)
                    return property.GetGetMethod(true)?.IsStatic ?? false
                        ? (MethodParams == null
                            ? (IEnumerable?)property.GetValue(null, null)
                            : ReturnErrorAsParameter(ParamGivenToProperty))
                        : ReturnErrorAsParameter(SourceMustBeStatic);

                var m = member as MethodInfo;
                if (m != null)
                    return m.IsStatic
                        ? (MethodParams == null || m.GetParameters().Length == MethodParams.Length
                            ? (IEnumerable?)m.Invoke(null, MethodParams)
                            : ReturnErrorAsParameter(NumberOfArgsDoesNotMatch))
                        : ReturnErrorAsParameter(SourceMustBeStatic);
            }

            return null;
        }

        private static IEnumerable ReturnErrorAsParameter(string errorMessage)
        {
            var parms = new TestCaseParameters();
            parms.RunState = RunState.NotRunnable;
            parms.Properties.Set(PropertyNames.SkipReason, errorMessage);
            return new TestCaseParameters[] { parms };
        }

        private const string SourceMustBeStatic =
            "The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.";
        private const string ParamGivenToField = "You have specified a data source field but also given a set of parameters. Fields cannot take parameters, " +
                                                 "please revise the 3rd parameter passed to the TestCaseSourceAttribute and either remove " +
                                                 "it or specify a method.";
        private const string ParamGivenToProperty = "You have specified a data source property but also given a set of parameters. " +
                                                    "Properties cannot take parameters, please revise the 3rd parameter passed to the " +
                                                    "TestCaseSource attribute and either remove it or specify a method.";
        private const string NumberOfArgsDoesNotMatch = "You have given the wrong number of arguments to the method in the TestCaseSourceAttribute" +
                                                        ", please check the number of parameters passed in the object is correct in the 3rd parameter for the " +
                                                        "TestCaseSourceAttribute and this matches the number of parameters in the target method and try again.";

        #endregion
    }
}