using System.Collections.Generic;

namespace Matr.Utilities.Test
{
    public partial class GenericFactory
    {
        /// <summary>
        /// Gets all currently registered services.
        /// </summary>
        /// <returns>List of tuples of service types and services.</returns>
        public List<(int, string)> GetRegisteredServices()
        {
            return new List<(int, string)>
            {
                (1, "one"),
                (2, "two"),
                (3, "three")
            };
        }
    }
}