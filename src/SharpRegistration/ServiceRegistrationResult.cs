using System;
using Microsoft.Extensions.DependencyInjection;

namespace SharpRegistration
{
    public class ServiceRegistrationResult
    {
        public ServiceRegistrationResult(Type service, Type implementation, ServiceLifetime lifetime)
        {
            Service = service;
            Implementation = implementation;
            Lifetime = lifetime;
        }
            
        public Type Service { get; }
            
        public Type Implementation { get; }
            
        public ServiceLifetime Lifetime { get; }
    }
}