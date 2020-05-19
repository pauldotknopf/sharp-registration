using System;
using Microsoft.Extensions.DependencyInjection;

namespace SharpRegistration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute()
        {
            
        }

        public ServiceAttribute(Type serviceType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            Lifetime = lifetime;
        }
        
        public Type ServiceType { get; }

        public ServiceLifetime Lifetime { get; } = ServiceLifetime.Singleton;
    }
}