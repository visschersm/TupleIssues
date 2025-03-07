using Autofac;
using System;
using System.Linq;

namespace Matr.Utilities.Test
{
    public partial class GenericFactory
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

        private ContainerBuilder CreateContainerBuilder(IContainer container)
        {
            var services = GetOwnServices(container);
            return CreateContainerBuilder(services);
        }
    }
}