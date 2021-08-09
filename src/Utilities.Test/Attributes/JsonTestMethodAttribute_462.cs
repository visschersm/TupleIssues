using System;
using System.Text;
using Newtonsoft.Json;
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

                var dataArray = JsonConvert.DeserializeObject(json, dataType.MakeArrayType());
                return (dataArray as IEnumerable).Cast<object>()
                    .Select(x => testMethod.Invoke(new object[] { x }))
                    .ToArray();
            }
        }
    }
}