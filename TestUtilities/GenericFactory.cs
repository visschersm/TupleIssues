using Autofac;
using System;
using System.Linq;

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

            containerBuilder.RegisterInstance(service);
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
                .Where(x => x.Activator.LimitType != typeof(TService))
                .Select(x => x.GetType());

            container = null;

            containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterTypes(components.ToArray());
        }

        /// <summary>
        /// Checks to see if a service of type TService is registered.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public virtual bool IsRegistered<T>()
            where T : class
        {
            if (container == null)
                container = containerBuilder.Build();

            return container.IsRegistered<T>();
        }

        public IContainer RebuildContainer<T>(IContainer container)
            where T : class
        {
            var components = container.ComponentRegistry.Registrations
                .Select(x => x.GetType());

            containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<T>()
                .PropertiesAutowired();
            containerBuilder.RegisterTypes(components.ToArray());

            return containerBuilder.Build();
        }
    }
}