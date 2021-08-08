using System;
using System.Text.Json;
using System.Linq;
using System.Collections;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Matr.Utilities.Test.Attributes
{
    [AttributeUsage(System.AttributeTargets.Method)]
    public class JsonTestMethodAttribute : TestMethodAttribute
    {
        private readonly string filepath;
        private readonly Type dataType;
        public JsonTestMethodAttribute(string filepath, Type dataType)
        {
            this.filepath = filepath;
            this.dataType = dataType;
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            using (StreamReader r = new StreamReader(filepath))
            {
                string json = r.ReadToEnd();

                var dataArray = JsonSerializer.Deserialize(json, dataType.MakeArrayType());
                return (dataArray as IEnumerable)!.Cast<object>()
                    .Select(x => testMethod.Invoke(new object[] { x }))
                    .ToArray();
            }
        }
    }
}