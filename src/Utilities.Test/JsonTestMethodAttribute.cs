using System;
#if NET462 || NET48 || NETSTANDARD20 || NETSTANDARD21
using System.Text;  
using Newtonsoft.Json;
#else
using System.Text.Json;
#endif
using System.Linq;
using System.Collections;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Matr.Utilities.Test
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

#if NET462 || NET48 || NETSTANDARD20 || NETSTANDARD21
                var dataArray = JsonConvert.DeserializeObject(json, dataType.MakeArrayType());
                return (dataArray as IEnumerable).Cast<object>()
#else
                var dataArray = JsonSerializer.Deserialize(json, dataType.MakeArrayType());
                return (dataArray as IEnumerable)!.Cast<object>()
#endif
                    .Select(x => testMethod.Invoke(new object[] { x }))
                    .ToArray();
            }
        }
    }
}