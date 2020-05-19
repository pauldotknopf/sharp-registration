using Microsoft.Extensions.DependencyInjection;

namespace SharpRegistration
{
    public interface IRegistrar
    {
        void Register(IServiceCollection serviceCollection);

        int Order { get; }
    }
}