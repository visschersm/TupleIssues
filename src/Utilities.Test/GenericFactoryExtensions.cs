using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Matr.Utilities.Test
{
    public static class GenericFactoryExtensions
    {
        /// <summary>
        /// Extension method to fluently register or replace services.
        /// </summary>
        /// <param name="source">Factory to register services to.</param>
        /// <param name="service">Service to register.</param>
        /// <typeparam name="TService">Type of the service to register.</typeparam>
        /// <returns>Instance of the factory that the service was registered to.</returns>
        public static GenericFactory RegisterOrReplace<TService>(this GenericFactory source, TService service)
            where TService : class
        {
            source.RegisterOrReplaceService(service);

            return source;
        }
    }
}