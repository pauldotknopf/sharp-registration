using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharpRegistration
{
    public interface IRegistrar
    {
        void Register(IServiceCollection serviceCollection, IConfiguration configuration);

        int Order { get; }
    }
}