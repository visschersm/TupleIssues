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
        private ContainerBuilder containerBuilder = new ContainerBuilder();
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
            var foo = typeof(TService);
            _ = service ?? throw new ArgumentNullException(nameof(service));

            if (IsRegistered<TService>())
                RemoveService<TService>();

            containerBuilder.RegisterInstance(service).As<TService>();
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

            var components = container.ComponentRegistry.Registrations
                //.Where(x => x.Activator.LimitType != typeof(ILifetimeScope))
                .SelectMany(x => x.Services)
                .Where(x => (x as TypedService).ServiceType != typeof(TService))
                .ToArray();

            var services = components.Select(x => new
            {
                ServiceType = (x as TypedService).ServiceType,
                Service = container.Resolve((x as TypedService).ServiceType)
            });

            containerBuilder = new ContainerBuilder();
            services.ToList().ForEach(x => containerBuilder.RegisterInstance(x.Service).As(x.ServiceType));

            container = null;
        }

        /// <summary>
        /// Checks to see if a service of type TService is registered.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public virtual bool IsRegistered<T>()
            where T : class
        {

            container = RebuildContainer();

            return container.IsRegistered<T>();
        }

        public IContainer RebuildContainer(IContainer container)
        {
            var components = container.ComponentRegistry.Registrations
                .SelectMany(x => x.Services)
                .ToArray();

            var services = components.Select(x => new
            {
                ServiceType = (x as TypedService).ServiceType,
                Service = container.Resolve((x as TypedService).ServiceType)
            });

            containerBuilder = new ContainerBuilder();
            services.ToList().ForEach(x => containerBuilder.RegisterInstance(x.Service).As(x.ServiceType));

            return containerBuilder.Build();
        }

        public IContainer RebuildContainer<T>(IContainer container)
            where T : class
        {
            var components = container.ComponentRegistry.Registrations
                .SelectMany(x => x.Services)
                .ToArray();

            var services = components.Select(x => new 
            { 
                ServiceType = (x as TypedService).ServiceType, 
                Service = container.Resolve((x as TypedService).ServiceType) 
            });

            containerBuilder = new ContainerBuilder();
            services.ToList().ForEach(x => containerBuilder.RegisterInstance(x.Service).As(x.ServiceType));

            containerBuilder.RegisterType<T>()
                .PropertiesAutowired();

            return containerBuilder.Build();
        }
    }
}