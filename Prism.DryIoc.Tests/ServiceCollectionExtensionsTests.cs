using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.DryIoc;
using Prism.Ioc;
using System.Collections.Generic;
using System.Linq;
using DryIoc.Microsoft.DependencyInjection.Extension;

namespace DryIoc.Microsoft.DependencyInjection.Extension.Prism.Dryioc.Tests
{
    [TestClass()]
    public class ServiceCollectionExtensionsTests
    {
        static Rules CreateContainerRules() => Rules.Default.WithAutoConcreteTypeResolution()
            .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
            .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);
        static IContainer CreateContainer()
            => new Container(CreateContainerRules());
        static IContainerExtension CreateContainerExtension()
            => new DryIocContainerExtension(CreateContainer());
        [TestMethod()]
        public void RegisterServices_Transient_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddTransient<IA>(v => new A1());
            });
            var Values = Container.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(1, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));

            var Value = Container.Resolve<IA>();
            Assert.IsInstanceOfType(Value, typeof(A1));
        }
        [TestMethod()]
        public void RegisterServices_Transient_Many_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddTransient<IA>(v => new A1());
                v.AddTransient<IA, A2>();
            });
            var Values = Container.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(2, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));
            Assert.IsInstanceOfType(Values[1], typeof(A2));
            Assert.ThrowsException<ContainerResolutionException>(() => Container.Resolve<IA>());
        }
        [TestMethod()]
        public void RegisterServices_Singleton_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddSingleton<IA>(v => new A1());
            });
            var Values = Container.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(1, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));

            var Value = Container.Resolve<IA>();
            Assert.IsInstanceOfType(Value, typeof(A1));
        }
        [TestMethod()]
        public void RegisterServices_Singleton_Many_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddSingleton<IA>(v => new A1());
                v.AddSingleton<IA, A2>();
            });
            var Values = Container.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(2, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));
            Assert.IsInstanceOfType(Values[1], typeof(A2));
            Assert.ThrowsException<ContainerResolutionException>(() => Container.Resolve<IA>());
        }
        [TestMethod()]
        public void RegisterServices_Scoped_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddScoped<IA>(v => new A1());
            });
            using var Scope = Container.CreateScope();
            var Values = Scope.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(1, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));

            var Value = Scope.Resolve<IA>();
            Assert.IsInstanceOfType(Value, typeof(A1));
        }
        [TestMethod()]
        public void RegisterServices_Scoped_Many_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddScoped<IA>(v => new A1());
                v.AddScoped<IA, A2>();
            });
            using var Scope = Container.CreateScope();
            var Values = Scope.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(2, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));
            Assert.IsInstanceOfType(Values[1], typeof(A2));

            Assert.ThrowsException<ContainerResolutionException>(() => Container.Resolve<IA>());
        }
        interface IA
        {
            string Value { get; set; }
        }
        class A1 : IA
        {
            public string Value { get; set; } = string.Empty;
        }
        class A2 : IA
        {
            public string Value { get; set; } = string.Empty;

        }
        [TestMethod()]
        public void RegisterServices_Transient_Generic_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddTransient(typeof(IB<>), typeof(B1<>));
            });
            var Values = Container.Resolve<IEnumerable<IB<string>>>().ToList();
            Assert.AreEqual(1, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(B1<string>));

            var Value = Container.Resolve<IB<string>>();
            Assert.IsInstanceOfType(Value, typeof(B1<string>));
        }
        [TestMethod()]
        public void RegisterServices_Transient_Generic_Many_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddTransient(typeof(IB<>), typeof(B1<>));
                v.AddTransient(typeof(IB<>), typeof(B2<>));
            });
            var Values = Container.Resolve<IEnumerable<IB<string>>>().ToList();
            Assert.AreEqual(2, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(B1<string>));
            Assert.IsInstanceOfType(Values[1], typeof(B2<string>));
            Assert.ThrowsException<ContainerResolutionException>(() => Container.Resolve<IB<string>>());
        }
        [TestMethod()]
        public void RegisterServices_Singleton_Generic_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddSingleton(typeof(IB<>), typeof(B1<>));
            });
            var Values = Container.Resolve<IEnumerable<IB<string>>>().ToList();
            Assert.AreEqual(1, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(B1<string>));

            var Value = Container.Resolve<IB<string>>();
            Assert.IsInstanceOfType(Value, typeof(B1<string>));
        }
        [TestMethod()]
        public void RegisterServices_Singleton_Generic_Many_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddSingleton(typeof(IB<>), typeof(B1<>));
                v.AddSingleton(typeof(IB<>), typeof(B2<>));
            });
            var Values = Container.Resolve<IEnumerable<IB<string>>>().ToList();
            Assert.AreEqual(2, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(B1<string>));
            Assert.IsInstanceOfType(Values[1], typeof(B2<string>));
            Assert.ThrowsException<ContainerResolutionException>(() => Container.Resolve<IB<string>>());
        }
        [TestMethod()]
        public void RegisterServices_Scoped_Generic_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddScoped(typeof(IB<>), typeof(B1<>));
            });
            using var Scope = Container.CreateScope();
            var Values = Scope.Resolve<IEnumerable<IB<string>>>().ToList();
            Assert.AreEqual(1, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(B1<string>));

            var Value = Scope.Resolve<IB<string>>();
            Assert.IsInstanceOfType(Value, typeof(B1<string>));
        }
        [TestMethod()]
        public void RegisterServices_Scoped_Generic_Many_Test()
        {
            var Container = CreateContainerExtension();
            IContainerRegistry Registry = Container;
            Registry.GetContainer().RegisterServices(v =>
            {
                v.AddScoped(typeof(IB<>), typeof(B1<>));
                v.AddScoped(typeof(IB<>), typeof(B2<>));
            });
            using var Scope = Container.CreateScope();
            var Values = Scope.Resolve<IEnumerable<IB<string>>>().ToList();
            Assert.AreEqual(2, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(B1<string>));
            Assert.IsInstanceOfType(Values[1], typeof(B2<string>));

            Assert.ThrowsException<ContainerResolutionException>(() => Container.Resolve<IB<string>>());
        }
        interface IB<T>
        {
            T Value { get; set; }
        }
        class B1<T> : IB<T>
        {
            public T Value { get; set; }
        }
        class B2<T> : IB<T>
        {
            public T Value { get; set; }
        }
    }
}