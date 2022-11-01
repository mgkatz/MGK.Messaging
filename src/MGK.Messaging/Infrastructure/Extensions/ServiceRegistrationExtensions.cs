using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UKU.Services.Infrastructure.Extensions;

namespace UKU.Messaging.Infrastructure.Extensions;

public static class ServiceRegistrationExtensions
{
    public static void AddMessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServicesInAssembly<IEmailClient>(configuration);
    }
}
