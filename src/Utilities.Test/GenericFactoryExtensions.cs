using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Matr.Utilities.Test
{
    public static class GenericFactoryExtensions
    {
        public static GenericFactory RegisterOrReplace<TService>(this GenericFactory source, TService service)
            where TService : class
        {
            source.RegisterOrReplaceService(service);

            return source;
        }
    }
}