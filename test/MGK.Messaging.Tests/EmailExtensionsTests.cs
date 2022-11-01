using UKU.Messaging.Infrastructure.Extensions;

namespace UKU.Messaging.Tests;

[TestFixture]
public class EmailExtensionsTests
{
    [TestCase("", false)]
    [TestCase(" ", false)]
    [TestCase(null, false)]
    [TestCase("test", false)]
    [TestCase("test@", false)]
    [TestCase("test@mail", false)]
    [TestCase("@mail.com", false)]
    [TestCase("test@mail.com", true)]
    public void MailClient_IsValidEmail_Should_ValidateEmailAddress(string emailAddress, bool isValidValue)
    {
        // Setup
        // Act
        bool isValidEmail = emailAddress.IsValidEmail();

        // Assert
        Assert.That(isValidEmail, Is.EqualTo(isValidValue));
    }
}
