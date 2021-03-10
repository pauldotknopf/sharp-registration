using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharpRegistration
{
    internal static class ServiceRegistration
    {
        public static void RegisterAllTypes(IServiceCollection services, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }
            
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass || type.IsAbstract)
                {
                    continue;
                }

                RegistrationContext.TryRegisterService(type, services);
            }
        }
        
        public static ServiceRegistrationResult Register(IServiceCollection services, ServiceAttribute attribute, Type declaredType, ServiceBuilderDelegate instantiate = null)
        {
            if (declaredType == null)
            {
                throw new NotSupportedException();
            }

            var serviceType = attribute.ServiceType;
            if (serviceType == null)
            {
                serviceType = declaredType;
            }

            if (declaredType.ContainsGenericParameters && serviceType.ContainsGenericParameters)
            {
                // Open generic type
                if (instantiate != null)
                {
                    throw new NotSupportedException("Cannot provide delegate for instantiation for open generic types.");
                }
            }
            else
            {
                if (declaredType.ContainsGenericParameters && !serviceType.ContainsGenericParameters)
                {
                    
                    // We are registering services on a generic type implementation
                    declaredType = declaredType.MakeGenericType(serviceType.GetGenericArguments());
                }
            }

            services.Add(instantiate != null
                ? new ServiceDescriptor(serviceType, provider => instantiate(provider, serviceType, declaredType),
                    attribute.Lifetime)
                : new ServiceDescriptor(serviceType, declaredType, attribute.Lifetime));

            return new ServiceRegistrationResult(serviceType, declaredType, attribute.Lifetime);
        }
    }
}