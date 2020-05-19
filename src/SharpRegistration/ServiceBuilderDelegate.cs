using System;

namespace SharpRegistration
{
    public delegate object ServiceBuilderDelegate(IServiceProvider provider, Type service, Type implementation);
}