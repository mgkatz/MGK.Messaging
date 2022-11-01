using MGK.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using UKU.Messaging.Infrastructure.Enums;
using UKU.Messaging.Infrastructure.Exceptions;
using UKU.Messaging.Infrastructure.Extensions;
using UKU.Messaging.Tests.Base;
using UKU.Messaging.Tests.Constants;
using UKU.Messaging.Tests.Models;

namespace UKU.Messaging.Tests;

[TestFixture]
public class MailClientTests : MessagingTestsBase
{
    private Mock<ILogger<MailClient>> _mockLogger;
    private MailClient _mailClient;
    private ISmtpObject _smtpObject;
    private TestOptionsModel _testOptions;

    [SetUp]
    public new void Setup()
    {
        base.Setup();

        _mockLogger = GetMockLogger<MailClient>();
        _smtpObject = new SmtpObject(MockSmtpIOptions.Object, GetMockLogger<SmtpObject>().Object);
        _mailClient = new MailClient(_smtpObject, MockSmtpIOptions.Object, _mockLogger.Object);

        Assert.That(TestConfiguration.GetSection(ConfigurationConstants.TestOptions.Key).Exists(), Is.True);

        _testOptions = TestConfiguration.GetSection(ConfigurationConstants.TestOptions.Key).Get<TestOptionsModel>();
        Assert.That(_testOptions.MailTo.IsValidEmail(), Is.True);
    }

    [Test]
    public async Task MailClient_SendMessageAsync_Should_SendEmail_When_AllIsFine()
    {
        // Setup
        var mailMessage = GetMailMessage();

        // Act
        ISendMessageResult result = await _mailClient.SendMessageAsync(mailMessage, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Status, Is.EqualTo(SendEmailStatus.Sent));
        Assert.That(result.Error, Is.Null);

        string expectedMessage = MessagingResources.MessagesResources.SendEmailSuccessful.Format(mailMessage.To.ToString());
        VerifyLoggerWithExactMessage(LogLevel.Information, expectedMessage, Times.Once());
    }

    [Test]
    public async Task MailClient_SendMessageAsync_Should_CancelEmail_When_IsRequested()
    {
        // Setup
        var mailMessage = GetMailMessage();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(1));

        // Act
        ISendMessageResult result = await _mailClient.SendMessageAsync(mailMessage, cancellationTokenSource.Token);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Status, Is.EqualTo(SendEmailStatus.Cancelled));
        Assert.That(result.Error, Is.Null);

        string expectedMessage = MessagingResources.MessagesResources.SendEmailCancelled;
        VerifyLoggerWithExactMessage(LogLevel.Warning, expectedMessage, Times.Once());
    }

    [Test]
    public async Task MailClient_SendMessageAsync_Should_FailSendingEmail_When_ErrorHappens()
    {
        // Setup
        var mailMessage = GetMailMessage();
        TestSmtpOptions.Client = "wrongsmtp.gmail.com";

        // Act
        ISendMessageResult result = await _mailClient.SendMessageAsync(mailMessage, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Status, Is.EqualTo(SendEmailStatus.Failed));
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error, Is.TypeOf<SendMessageException>());
        Assert.That(result.NumberOfRetriesMade, Is.EqualTo(TestSmtpOptions.Retries));

        var expectedErrorMessage = MessagingResources.MessagesResources.SendEmailError.Format(mailMessage.To.ToString());
        VerifyLoggerWithExactMessage(LogLevel.Error, expectedErrorMessage, Times.Once());
    }

    [Test]
    public void MailClient_SendMessageAsync_Should_Fail_When_NoMailMessageIsProvided()
    {
        // Setup
        // Act
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _mailClient.SendMessageAsync(null, CancellationToken.None));
        Assert.That(_mockLogger.Invocations, Is.Empty);
    }

    private MailMessage GetMailMessage()
    {
        return new MailMessage(
            from: TestSmtpOptions.UserName,
            to: _testOptions.MailTo,
            subject: "Unit test de envío de mails de UKU",
            body: "Este es un envío de mail de pruebas de UKU.");
    }

    private void VerifyLoggerWithExactMessage(
        LogLevel logLevel,
        string logMessage,
        Times numberOfInvocations)
    {
        _mockLogger.Verify(x =>
            x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals(logMessage, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                numberOfInvocations);
    }
}
