using Autofac;
using System;
using System.Linq;
using Autofac.Core;
using System.Collections.Generic;

namespace Matr.Utilities.Test
{
    public partial class GenericFactory
    {
        /// <summary>
        /// Gets all currently registered services.
        /// </summary>
        /// <returns>List of tuples of service types and services.</returns>
        public List<Tuple<Type, object>> GetRegisteredServices()
        {
            return GetOwnServices(container).Select(x => new Tuple<Type, object>(x.Item1, x.Item2)).ToList();
        }

        private (Type ServiceType, object Service)[] GetOwnServices(IContainer container)
        {
            return container.ComponentRegistry.Registrations
                 .SelectMany(x => x.Services)
                 .Where(x => (x as TypedService) != null)
                 .Where(x => (x as TypedService).ServiceType != typeof(ILifetimeScope))
                 .Where(x => (x as TypedService).ServiceType != typeof(IComponentContext))
                 .Select(x => ((x as TypedService).ServiceType, TryResolve(x)))
                 .ToArray();
        }

        private object TryResolve(Service x)
        {
            try
            {

                return container.Resolve((x as TypedService).ServiceType);
            }
            catch (DependencyResolutionException)
            {
                return null;
            }
        }

        // TODO: Should Service not be lowercase here?
        private static ContainerBuilder CreateContainerBuilder((Type ServiceType, object Service)[] services)
        {
            var containerBuilder = new ContainerBuilder();

            services.Where(x => x.Service == null)
                .ToList()
                .ForEach(x => containerBuilder.RegisterType(x.ServiceType));

            services.Where(x => x.Service != null)
                      .ToList()
                      .ForEach(x => containerBuilder.RegisterInstance(x.Service).As(x.ServiceType));
            return containerBuilder;
        }
    }
}