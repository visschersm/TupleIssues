using System;
using System.Text.Json;
using System.Linq;
using System.Collections;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Matr.Utilities.Test.Attributes
{
    public partial class JsonTestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            using (StreamReader r = new StreamReader(filepath))
            {
                string json = r.ReadToEnd();

                var dataArray = JsonSerializer.Deserialize(json, dataType.MakeArrayType());
#if NET48
                return (dataArray as IEnumerable).Cast<object>()
#else
                return (dataArray as IEnumerable)!.Cast<object>()
#endif
                    .Select(x => testMethod.Invoke(new object[] { x }))
                    .ToArray();
            }
        }
    }
}