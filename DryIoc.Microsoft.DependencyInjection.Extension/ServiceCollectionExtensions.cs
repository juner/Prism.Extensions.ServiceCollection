using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DryIoc.Microsoft.DependencyInjection.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IContainer RegisterServices(this IContainer container, Action<IServiceCollection> Action, Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null)
        {
            var descriptors = new ServiceCollection();
            Action.Invoke(descriptors);
            DependencyInjectionAdapter(container, descriptors, registerDescriptor);
            return container;
        }
        static void DependencyInjectionAdapter(IContainer container, IEnumerable<ServiceDescriptor> descriptors = null,
            Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null)
        {
            container.Use<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));
            // Registers service collection
            if (descriptors != null)
                Populate(container, descriptors, registerDescriptor);
            var Provider = container.BuildServiceProvider();
            container.RegisterInstance(Provider);
        }
        static void Populate(IContainer container, IEnumerable<ServiceDescriptor> descriptors, Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null)
        {
            var d = descriptors.GroupBy(v => v.ServiceType).Select(v => (ServiceType: v.Key, Descriptors: v.ToList()));
            if (registerDescriptor is null)
                foreach (var (serviceType, ds) in d)
                    if (ds.Count == 1)
                        RegisterDescriptor(container, ds.First());
                    else
                        foreach (var descriptor in ds)
                            RegisterDescriptorMany(container, descriptor);
            else
                foreach (var (serviceType, ds) in d)
                    if (ds.Count == 1)
                    {
                        if (!registerDescriptor(container, ds.First()))
                            RegisterDescriptor(container, ds.First());
                    }
                    else
                    {
                        foreach (var descriptor in ds)
                            if (!registerDescriptor(container, descriptor))
                                RegisterDescriptorMany(container, descriptor);
                    }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="descriptor"></param>
        static void RegisterDescriptorMany(IContainer container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType is { })
            {
                var reuse = descriptor.Lifetime switch
                {
                    ServiceLifetime.Singleton => Reuse.Singleton,
                    ServiceLifetime.Scoped => Reuse.ScopedOrSingleton,
                    _ => Reuse.Transient
                };
                container.Register(descriptor.ServiceType, descriptor.ImplementationType, reuse, ifAlreadyRegistered: IfAlreadyRegistered.AppendNewImplementation);
            }
            else if (descriptor.ImplementationFactory is { })
            {
                var reuse = descriptor.Lifetime switch
                {
                    ServiceLifetime.Singleton => Reuse.Singleton,
                    ServiceLifetime.Scoped => Reuse.ScopedOrSingleton,
                    _ => Reuse.Transient
                };
                container.RegisterDelegate(true, descriptor.ServiceType, descriptor.ImplementationFactory, reuse, ifAlreadyRegistered: IfAlreadyRegistered.AppendNewImplementation);
            }
            else
            {
                container.RegisterInstance(true, descriptor.ServiceType, descriptor.ImplementationInstance, ifAlreadyRegistered: IfAlreadyRegistered.AppendNewImplementation);
            }
        }
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
                container.Register(descriptor.ServiceType, descriptor.ImplementationType, reuse);
            }
            else if (descriptor.ImplementationFactory is { })
            {
                var reuse = descriptor.Lifetime switch
                {
                    ServiceLifetime.Singleton => Reuse.Singleton,
                    ServiceLifetime.Scoped => Reuse.ScopedOrSingleton,
                    _ => Reuse.Transient
                };
                container.RegisterDelegate(true, descriptor.ServiceType, descriptor.ImplementationFactory, reuse);
            }
            else
            {
                container.RegisterInstance(true, descriptor.ServiceType, descriptor.ImplementationInstance);
            }
        }
    }
}
