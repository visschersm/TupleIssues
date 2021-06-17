using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Decorators;
using Autofac.Core;
using Autofac.Builder;
using Autofac.Core.Lifetime;

namespace MatrTech.Utility.Test
{
    public class GenericFactory
    {
        private IContainer container;

        public GenericFactory()
        {
            var containerBuilder = new ContainerBuilder();
            container = containerBuilder.Build();
        }

        /// <summary>
        /// Creates a new instance of T resolving its dependencies.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Create<T>()
            where T : class
        {
            RemoveService<T>();
            var containerBuilder = CreateContainerBuilder(container);

            containerBuilder.RegisterType<T>()
                .PropertiesAutowired();

            container = containerBuilder.Build();
            return container.Resolve<T>();
        }

        /// <summary>
        /// Replaces the registered service if a service of the given type is already registered.
        /// Otherwise registers a new instance of the type.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="service"></param>
        public virtual void RegisterOrReplaceService<TService>(TService service)
            where TService : class
        {
            _ = service ?? throw new ArgumentNullException(nameof(service));

            var containerBuilder = CreateContainerBuilder(container);
            RemoveService<TService>();

            containerBuilder.RegisterInstance(service)
                .As<TService>()
                .PropertiesAutowired();

            container = containerBuilder.Build();
        }

        /// <summary>
        /// Clears out and resets the containerBuilder.
        /// </summary>
        public virtual void EmptyDependencies()
        {
            container = new ContainerBuilder().Build();
        }

        /// <summary>
        /// Removes a service of type TService.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public virtual void RemoveService<TService>()
        {
            var services = GetOwnServices(container)
                .Where(x => x.ServiceType != typeof(TService))
                .ToArray();

            container = CreateContainerBuilder(services).Build();
        }

        /// <summary>
        /// Checks to see if a service of type TService is registered.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public virtual bool IsRegistered<T>()
            where T : class
        {
            return container.IsRegistered<T>();
        }

        private (Type ServiceType, object? Service)[] GetOwnServices(IContainer container)
        {
            return container.ComponentRegistry.Registrations
                 .SelectMany(x => x.Services)
                 .Where(x => (x as TypedService) != null)
                 .Where(x => (x as TypedService)!.ServiceType != typeof(ILifetimeScope))
                 .Where(x => (x as TypedService)!.ServiceType != typeof(IComponentContext))
                 .Select(x => ((x as TypedService)!.ServiceType, TryResolve(x)))
                 .ToArray();
        }

        private object? TryResolve(Service x)
        {
            try
            {
                return container.Resolve((x as TypedService)!.ServiceType);
            }
            catch (DependencyResolutionException)
            {
                return null;
            }
        }

        private ContainerBuilder CreateContainerBuilder(IContainer container)
        {
            var services = GetOwnServices(container);
            return CreateContainerBuilder(services);
        }

        private static ContainerBuilder CreateContainerBuilder((Type ServiceType, object? Service)[] services)
        {
            var containerBuilder = new ContainerBuilder();

            services.Where(x => x.Service == null)
                .ToList()
                .ForEach(x => containerBuilder.RegisterType(x.ServiceType));

            services.Where(x => x.Service != null)
                .ToList()
                .ForEach(x => containerBuilder.RegisterInstance(x.Service!).As(x.ServiceType));

            return containerBuilder;
        }
    }
}