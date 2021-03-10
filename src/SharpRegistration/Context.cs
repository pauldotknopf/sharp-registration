using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharpRegistration
{
    public static class RegistrationContext
    {
        private static readonly object Lock = new object();

        public static IServiceProvider Provider { get; private set; }

        public static T Get<T>()
        {
            return Provider.GetRequiredService<T>();
        }

        public static object Get(Type type)
        {
            return Provider.GetRequiredService(type);
        }

        public static void Register(IConfiguration configuration, params IRegistrar[] registrars)
        {
            lock (Lock)
            {
                if (Provider != null)
                    return;

                Provider = Build(null, configuration, registrars.ToArray());
            }
        }

        public static IServiceProvider Build(IServiceCollection services, IConfiguration configuration, params IRegistrar[] registrars)
        {
            if (services == null)
            {
                services = new ServiceCollection();
            }

            foreach (var registrar in registrars.OrderBy(x => x.Order))
            {
                registrar.Register(services, configuration);
            }
                
            return services.BuildServiceProvider();
        }
        
        public static IRegistrar Registrar(int order, Action<IServiceCollection> action)
        {
            return new DelegateRegistrar(order, action);
        }

        private class DelegateRegistrar : IRegistrar
        {
            private readonly Action<IServiceCollection> _action;

            public DelegateRegistrar(int order, Action<IServiceCollection> action)
            {
                Order = order;
                _action = action;
            }

            public int Order { get; }

            public void Register(IServiceCollection services, IConfiguration configuration)
            {
                _action(services);
            }
        }

        public static List<ServiceRegistrationResult> TryRegisterService(Type type, IServiceCollection services, ServiceBuilderDelegate instantiate = null)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                return null;
            }

            var serviceAttributes = type.GetCustomAttributes(typeof(ServiceAttribute), false)
                .OfType<ServiceAttribute>().ToList();
            List<ServiceRegistrationResult> results = null;
            
            foreach (var serviceAttribute in serviceAttributes)
            {
                if (results == null)
                {
                    results = new List<ServiceRegistrationResult>();
                }
                results.Add(ServiceRegistration.Register(services, serviceAttribute, type, instantiate));
            }

            return results;
        }
    }
}