using Autofac;
using System;
using System.Linq;

namespace MPTech.TestUtilities
{
    public class GenericFactory
    {
        protected ContainerBuilder containerBuilder = new ContainerBuilder();
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
            this.containerBuilder.RegisterType<T>()
                .PropertiesAutowired();

            if (this.container != null)
                throw new Exception();

            this.container = this.containerBuilder.Build();

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
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            this.containerBuilder.RegisterInstance(service);
        }

        /// <summary>
        /// Clears out and resets the containerBuilder.
        /// </summary>
        public virtual void EmptyDependencies()
        {
            this.containerBuilder = new ContainerBuilder();
        }

        /// <summary>
        /// Removes a service of type TService.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public virtual void RemoveService<TService>()
        {
            if (this.container == null)
                this.container = this.containerBuilder.Build();

            var components = this.container.ComponentRegistry.Registrations
                .Where(x => x.Activator.LimitType != typeof(TService))
                .Select(x => x.GetType());

            this.container = null;

            this.containerBuilder = new ContainerBuilder();

            this.containerBuilder.RegisterTypes(components.ToArray());
        }

        /// <summary>
        /// Checks to see if a service of type TService is registered.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public virtual bool IsRegistered<TService>()
            where TService : class
        {
            if (this.container == null)
                this.container = this.containerBuilder.Build();

            return this.container.IsRegistered<TService>();
        }
    }
}