using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Matr.Utilities.Test.Attributes
{
    [AttributeUsage(System.AttributeTargets.Method)]
    public partial class JsonTestMethodAttribute : TestMethodAttribute
    {
        private readonly string filepath;
        private readonly Type dataType;
        public JsonTestMethodAttribute(string filepath, Type dataType)
        {
            _ = File.Exists(filepath) ? filepath : throw new FileNotFoundException(filepath);
            _ = dataType ?? throw new ArgumentNullException(nameof(dataType));

            this.filepath = filepath;
            this.dataType = dataType;
        }
    }
}