using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DryIoc.Microsoft.DependencyInjection.Extension.Tests
{
    [TestClass()]
    public class ServiceCollectionExtensionsTests
    {
        static Rules CreateContainerRules() => Rules.Default.WithAutoConcreteTypeResolution()
            .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
            .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);
        static IContainer CreateContainer()
            => new Container(CreateContainerRules());
        [TestMethod()]
        public void RegisterServices_Transient_Test()
        {
            var Container = CreateContainer();
            Container.RegisterServices(v =>
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
            var Container = CreateContainer();
            Container.RegisterServices(v =>
            {
                v.AddTransient<IA>(v => new A1());
                v.AddTransient<IA, A2>();
            });
            var Values = Container.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(2, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));
            Assert.IsInstanceOfType(Values[1], typeof(A2));
            Assert.ThrowsException<ContainerException>(() => Container.Resolve<IA>());
        }
        [TestMethod()]
        public void RegisterServices_Singleton_Test()
        {
            var Container = CreateContainer();
            Container.RegisterServices(v =>
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
            var Container = CreateContainer();
            Container.RegisterServices(v =>
            {
                v.AddSingleton<IA>(v => new A1());
                v.AddSingleton<IA, A2>();
            });
            var Values = Container.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(2, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));
            Assert.IsInstanceOfType(Values[1], typeof(A2));
            Assert.ThrowsException<ContainerException>(() => Container.Resolve<IA>());
        }
        [TestMethod()]
        public void RegisterServices_Scoped_Test()
        {
            var Container = CreateContainer();
            Container.RegisterServices(v =>
            {
                v.AddScoped<IA>(v => new A1());
            });
            using var Scope = Container.OpenScope();
            var Values = Scope.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(1, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));

            var Value = Scope.Resolve<IA>();
            Assert.IsInstanceOfType(Value, typeof(A1));
        }
        [TestMethod()]
        public void RegisterServices_Scoped_Many_Test()
        {
            var Container = CreateContainer();
            Container.RegisterServices(v =>
            {
                v.AddScoped<IA>(v => new A1());
                v.AddScoped<IA, A2>();
            });
            using var Scope = Container.OpenScope();
            var Values = Scope.Resolve<IEnumerable<IA>>().ToList();
            Assert.AreEqual(2, Values.Count);
            Assert.IsInstanceOfType(Values[0], typeof(A1));
            Assert.IsInstanceOfType(Values[1], typeof(A2));

            Assert.ThrowsException<ContainerException>(() => Container.Resolve<IA>());
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
    }
}