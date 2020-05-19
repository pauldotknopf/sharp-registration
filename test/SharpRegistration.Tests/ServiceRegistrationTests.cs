using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace SharpRegistration.Tests
{
    public class ServiceRegistrationTests
    {
        private readonly Mock<InstantiateTestHarness> _mock;

        public ServiceRegistrationTests()
        {
            _mock = new Mock<InstantiateTestHarness>();
            _mock.CallBase = true;
        }
        
        public interface IService
        {
        }

        public class Service1 : IService
        {
            
        }
        
        [Fact]
        public void Can_register_service()
        {
            var services = new ServiceCollection();

            ServiceRegistration.Register(services, new ServiceAttribute(typeof(IService)), typeof(Service1),
                _mock.Object.Instantiate);
           
            services.Count.Should().Be(1); 
            services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);

            var resolved = services.BuildServiceProvider().GetRequiredService<IService>();
            resolved.Should().BeAssignableTo<IService>();
            resolved.Should().BeAssignableTo<Service1>();
            
            _mock.Verify(x => x.Instantiate(It.IsAny<IServiceProvider>(), typeof(IService), typeof(Service1)), Times.Once);
        }
        
        [Fact]
        public void Can_register_service_scoped()
        {
            var services = new ServiceCollection();

            ServiceRegistration.Register(services, new ServiceAttribute(typeof(IService), ServiceLifetime.Scoped),
                typeof(Service1), _mock.Object.Instantiate);
           
            services.Count.Should().Be(1); 
            services[0].Lifetime.Should().Be(ServiceLifetime.Scoped);

            var resolved = services.BuildServiceProvider().GetRequiredService<IService>();
            resolved.Should().BeAssignableTo<IService>();
            resolved.Should().BeAssignableTo<Service1>();
            
            _mock.Verify(x => x.Instantiate(It.IsAny<IServiceProvider>(), typeof(IService), typeof(Service1)), Times.Once);
        }
        
        [Fact]
        public void Can_register_service_transient()
        {
            var services = new ServiceCollection();

            ServiceRegistration.Register(services, new ServiceAttribute(typeof(IService), ServiceLifetime.Transient),
                typeof(Service1), _mock.Object.Instantiate);
           
            services.Count.Should().Be(1); 
            services[0].Lifetime.Should().Be(ServiceLifetime.Transient);

            var resolved = services.BuildServiceProvider().GetRequiredService<IService>();
            resolved.Should().BeAssignableTo<IService>();
            resolved.Should().BeAssignableTo<Service1>();
            
            _mock.Verify(x => x.Instantiate(It.IsAny<IServiceProvider>(), typeof(IService), typeof(Service1)), Times.Once);
        }
        
        [Fact]
        public void Can_declare_service_without_interface()
        {
            var services = new ServiceCollection();
            ServiceRegistration.Register(services, new ServiceAttribute(ServiceLifetime.Singleton), typeof(Service1), _mock.Object.Instantiate);

            services.Should().HaveCount(1);
            services[0].Lifetime.Should().Be(ServiceLifetime.Singleton);
            services[0].ServiceType.Should().Be(typeof(Service1));
            services[0].ImplementationType.Should().BeNull();
            
            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<Service1>();
            
            _mock.Verify(x => x.Instantiate(It.IsAny<IServiceProvider>(), typeof(Service1), typeof(Service1)), Times.Once);
        }
        
        [Fact]
        public void Can_declare_service_without_interface_scoped()
        {
            var services = new ServiceCollection();
            ServiceRegistration.Register(services, new ServiceAttribute(ServiceLifetime.Scoped), typeof(Service1), _mock.Object.Instantiate);

            services.Should().HaveCount(1);
            services[0].Lifetime.Should().Be(ServiceLifetime.Scoped);
            services[0].ServiceType.Should().Be(typeof(Service1));
            services[0].ImplementationType.Should().BeNull();
            
            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<Service1>();
            
            _mock.Verify(x => x.Instantiate(It.IsAny<IServiceProvider>(), typeof(Service1), typeof(Service1)), Times.Once);
        }
        
        [Fact]
        public void Can_declare_service_without_interface_transient()
        {
            var services = new ServiceCollection();
            ServiceRegistration.Register(services, new ServiceAttribute(ServiceLifetime.Transient), typeof(Service1), _mock.Object.Instantiate);

            services.Should().HaveCount(1);
            services[0].Lifetime.Should().Be(ServiceLifetime.Transient);
            services[0].ServiceType.Should().Be(typeof(Service1));
            services[0].ImplementationType.Should().BeNull();
            
            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<Service1>();
            
            _mock.Verify(x => x.Instantiate(It.IsAny<IServiceProvider>(), typeof(Service1), typeof(Service1)), Times.Once);
        }
        
        public interface IGenericService<T>
        {
            
        }

        public class GenericService<T> : IGenericService<T>
        {
            
        }

        [Fact]
        public void Can_register_generic_services()
        {
            var services = new ServiceCollection();
            
            ServiceRegistration.Register(services, new ServiceAttribute(typeof(IGenericService<string>)), typeof(GenericService<>), _mock.Object.Instantiate);
            ServiceRegistration.Register(services, new ServiceAttribute(typeof(IGenericService<int>)), typeof(GenericService<>), _mock.Object.Instantiate);

            services.Should().HaveCount(2);

            services[0].ServiceType.Should().Be(typeof(IGenericService<string>));
            services[1].ServiceType.Should().Be(typeof(IGenericService<int>));

            var serviceProvider = services.BuildServiceProvider();

            var service1 = serviceProvider.GetRequiredService<IGenericService<string>>();
            service1.Should().BeAssignableTo<GenericService<string>>();
            var service2 = serviceProvider.GetRequiredService<IGenericService<int>>();
            service2.Should().BeAssignableTo<GenericService<int>>();
        }

        [Fact]
        public void Can_register_open_generics()
        {
            var services = new ServiceCollection();

            ServiceRegistration.Register(services, new ServiceAttribute(typeof(IGenericService<>)), typeof(GenericService<>));

            services.Should().HaveCount(1);

            services[0].ServiceType.Should().Be(typeof(IGenericService<>));

            var serviceProvider = services.BuildServiceProvider();

            var service1 = serviceProvider.GetRequiredService<IGenericService<string>>();
            service1.Should().BeAssignableTo<GenericService<string>>();
            var service2 = serviceProvider.GetRequiredService<IGenericService<int>>();
            service2.Should().BeAssignableTo<GenericService<int>>();
        }

        [Fact]
        public void Cant_register_open_generics_with_instantiation_delegate()
        {
            var services = new ServiceCollection();

            var exception = Assert.Throws<NotSupportedException>(() =>
                ServiceRegistration.Register(services, new ServiceAttribute(typeof(IGenericService<>)),
                    typeof(GenericService<>), (provider, service, implementation) => new object()));

            exception.Message.Should().Be("Cannot provide delegate for instantiation for open generic types.");
        }
        
        public class InstantiateTestHarness
        {
            public virtual object Instantiate(IServiceProvider provider, Type serviceType, Type implementation)
            {
                return Activator.CreateInstance(implementation);
            }
        }
    }
}