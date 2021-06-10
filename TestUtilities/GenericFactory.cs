using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Decorators;
using Autofac.Core;
using Autofac.Builder;
using Autofac.Core.Lifetime;

namespace MPTech.TestUtilities
{
    public class GenericFactory
    {
        public ContainerBuilder containerBuilder = new ContainerBuilder();
        private IContainer? container = null;

        public GenericFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of T resolving its dependencies.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Create<T>()
            where T : class
        {
            var foo = typeof(T);
            containerBuilder.RegisterType<T>()
                .PropertiesAutowired();

            if (container == null)
            {
                container = containerBuilder.Build();
            }
            else
            {
                container = RebuildContainer<T>(this.container);
            }

            return this.container.Resolve<T>();
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

            if (typeof(TService).IsInterface)
            {
                containerBuilder.RegisterInstance(service).As<TService>();
            }
            else
            {
                containerBuilder.RegisterInstance(service);
            }
        }

        /// <summary>
        /// Clears out and resets the containerBuilder.
        /// </summary>
        public virtual void EmptyDependencies()
        {
            containerBuilder = new ContainerBuilder();
        }

        /// <summary>
        /// Removes a service of type TService.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public virtual void RemoveService<TService>()
        {
            if (container == null)
                container = containerBuilder.Build();
            else
                container = RebuildContainer(container);

            var services = GetOwnServices(container)
                .Where(x => x.ServiceType != typeof(TService))
                .ToArray();

            containerBuilder = new ContainerBuilder();

            services.Where(x => x.ServiceType.IsInterface)
                .Where(x => x.ServiceType != typeof(ILifetimeScope))
                .Where(x => x.ServiceType != typeof(IComponentContext))
                .ToList()
                .ForEach(x => containerBuilder.RegisterInstance(x).As(x.ServiceType));

            services.Where(x => !x.ServiceType.IsInterface)
                .ToList()
                .ForEach(x => containerBuilder.RegisterInstance(x.Service));

            foreach (var c in services)
            {
                Console.WriteLine(c);
            }
            container = containerBuilder.Build();

        }

        /// <summary>
        /// Checks to see if a service of type TService is registered.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public virtual bool IsRegistered<T>()
            where T : class
        {
            if (container != null)
                container = RebuildContainer(container);
            else
                container = containerBuilder.Build();

            return container.IsRegistered<T>();
        }

        public IContainer RebuildContainer(IContainer container)
        {
            var serivces = GetOwnServices(container);

            containerBuilder = new ContainerBuilder();

            serivces.Where(x => x.ServiceType.IsInterface)
               .ToList()
               .ForEach(x => containerBuilder.RegisterInstance(x).As(x.ServiceType));

            serivces.Where(x => !x.ServiceType.IsInterface)
                .ToList()
                .ForEach(x => containerBuilder.RegisterInstance(x.Service));

            return containerBuilder.Build();
        }

        public IContainer RebuildContainer<T>(IContainer container)
            where T : class
        {
            var services = GetOwnServices(container);

            containerBuilder = new ContainerBuilder();

            services.Where(x => x.ServiceType.IsInterface)
               .ToList()
               .ForEach(x => containerBuilder.RegisterInstance(x).As(x.ServiceType));

            services.Where(x => !x.ServiceType.IsInterface)
                .ToList()
                .ForEach(x => containerBuilder.RegisterInstance(x.Service));

            containerBuilder.RegisterType<T>()
                .PropertiesAutowired();

            return containerBuilder.Build();
        }

        private ServiceTuple[] GetOwnServices(IContainer container)
        {
            return container.ComponentRegistry.Registrations
                 .Where(x => x.Activator.LimitType != typeof(ILifetimeScope))
                 .Where(x => x.Activator.LimitType != typeof(IComponentContext))
                 .SelectMany(x => x.Services)
                 .Select(x => new ServiceTuple
                 {
                     ServiceType = (x as TypedService).ServiceType,
                     Service = container.Resolve((x as TypedService).ServiceType)
                 })
                 .ToArray();
        }

        private class ServiceTuple
        {
            public Type ServiceType { get; set; }
            public object Service { get; set; }
        }
    }
}