using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DryIoc.Microsoft.DependencyInjection.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IContainer RegisterServices(this IContainer container, Action<IServiceCollection> Action, Func<IRegistrator, ServiceDescriptor, bool>? registerDescriptor = null)
        {
            var descriptors = new ServiceCollection();
            Action.Invoke(descriptors);
            DependencyInjectionAdapter(container, descriptors, registerDescriptor);
            return container;
        }
        static void DependencyInjectionAdapter(IContainer container, IEnumerable<ServiceDescriptor>? descriptors = null,
            Func<IRegistrator, ServiceDescriptor, bool>? registerDescriptor = null)
        {
            container.Use<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));
            // Registers service collection
            if (descriptors != null)
                Populate(container, descriptors, registerDescriptor);
            var Provider = container.BuildServiceProvider();
            container.RegisterInstance(Provider);
        }
        /// <summary>
        /// Populate <paramref name="container"/> with <paramref name="descriptors"/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="descriptors"></param>
        /// <param name="registerDescriptor"></param>
        static void Populate(IContainer container, IEnumerable<ServiceDescriptor> descriptors, Func<IRegistrator, ServiceDescriptor, bool>? registerDescriptor = null)
        {
            if (registerDescriptor is null)
                foreach (var descriptor in descriptors)
                    RegisterDescriptor(container, descriptor);
            else
                foreach (var descriptor in descriptors)
                    if (!registerDescriptor(container, descriptor))
                        RegisterDescriptor(container, descriptor);
        }
        /// <summary>
        /// Register the <paramref name="descriptor"/> in <paramref name="container"/>.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="descriptor"></param>
        static void RegisterDescriptor(IContainer container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType is { })
            {
                var reuse = descriptor.Lifetime switch
                {
                    ServiceLifetime.Singleton => Reuse.Singleton,
                    ServiceLifetime.Scoped => Reuse.ScopedOrSingleton,
                    _ => Reuse.Transient
                };
                container.Register(descriptor.ServiceType, descriptor.ImplementationType, reuse, ifAlreadyRegistered: IfAlreadyRegistered.AppendNotKeyed);
            }
            else if (descriptor.ImplementationFactory is { })
            {
                var reuse = descriptor.Lifetime switch
                {
                    ServiceLifetime.Singleton => Reuse.Singleton,
                    ServiceLifetime.Scoped => Reuse.ScopedOrSingleton,
                    _ => Reuse.Transient
                };
                container.RegisterDelegate(true, descriptor.ServiceType, descriptor.ImplementationFactory, reuse, ifAlreadyRegistered: IfAlreadyRegistered.AppendNotKeyed);
            }
            else
            {
                container.RegisterInstance(true, descriptor.ServiceType, descriptor.ImplementationInstance, ifAlreadyRegistered: IfAlreadyRegistered.AppendNotKeyed);
            }
        }
    }
}
