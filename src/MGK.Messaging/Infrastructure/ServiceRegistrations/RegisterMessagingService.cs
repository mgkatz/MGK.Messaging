using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UKU.Messaging.Infrastructure.Options;

namespace UKU.Messaging.Infrastructure.ServiceRegistrations;

public class RegisterMessagingService : IServiceRegistration
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.OptionsKey));
        services.TryAddScoped<IEmailClient, MailClient>();
        services.TryAddSingleton<ISmtpObject, SmtpObject>();
    }
}
