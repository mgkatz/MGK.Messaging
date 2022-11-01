using System.Net;
using UKU.Messaging.Tests.Base;

namespace UKU.Messaging.Tests;

[TestFixture]
public class SmtpObjectTests : MessagingTestsBase
{
    private SmtpObject _smtpObject;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        _smtpObject = new SmtpObject(MockSmtpIOptions.Object, GetMockLogger<SmtpObject>().Object);
    }

    [Test]
    public void SmtpObject_Should_GetClientAndMatchWithAppSettingsOptions()
    {
        // Setup
        // Act
        var smtpClient = _smtpObject.Client;

        // Assert
        Assert.That(smtpClient, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smtpClient.DeliveryMethod, Is.EqualTo(SmtpDeliveryMethod.Network));
            Assert.That(smtpClient.EnableSsl, Is.EqualTo(TestSmtpOptions.EnableSsl));
            Assert.That(smtpClient.Host, Is.EqualTo(TestSmtpOptions.Client));
            Assert.That(smtpClient.Port, Is.EqualTo(TestSmtpOptions.Port));
            Assert.That(smtpClient.Timeout, Is.EqualTo(TestSmtpOptions.Timeout));
            Assert.That(!smtpClient.UseDefaultCredentials, Is.True);
            Assert.That(smtpClient.Credentials, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(smtpClient.Credentials is NetworkCredential, Is.True);
            Assert.That((smtpClient.Credentials as NetworkCredential)?.UserName.Equals(TestSmtpOptions.UserName, StringComparison.Ordinal), Is.True);
            Assert.That((smtpClient.Credentials as NetworkCredential)?.Password.Equals(TestSmtpOptions.Password, StringComparison.Ordinal), Is.True);
        });
    }
}