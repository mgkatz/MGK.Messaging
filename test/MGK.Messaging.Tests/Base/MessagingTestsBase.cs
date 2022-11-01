using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using UKU.Messaging.Infrastructure.Options;
using UKU.Messaging.Tests.Constants;

namespace UKU.Messaging.Tests.Base;

public abstract class MessagingTestsBase
{
    protected Mock<IOptions<SmtpOptions>> MockSmtpIOptions { get; private set; }
    protected IConfigurationRoot TestConfiguration { get; private set; }
    protected SmtpOptions TestSmtpOptions { get; private set; }

    protected void Setup()
    {
        FileAssert.Exists(ConfigurationConstants.AppSettingsFile);
        TestConfiguration = new ConfigurationBuilder().AddJsonFile(ConfigurationConstants.AppSettingsFile).Build();

        Assert.That(TestConfiguration.GetSection(SmtpOptions.OptionsKey).Exists(), Is.True);
        TestSmtpOptions = TestConfiguration.GetSection(SmtpOptions.OptionsKey).Get<SmtpOptions>();

        MockSmtpIOptions = new Mock<IOptions<SmtpOptions>>();
        MockSmtpIOptions.Setup(opt => opt.Value).Returns(TestSmtpOptions);
    }

    protected Mock<ILogger<T>> GetMockLogger<T>() where T : class
        => new Mock<ILogger<T>>();
}