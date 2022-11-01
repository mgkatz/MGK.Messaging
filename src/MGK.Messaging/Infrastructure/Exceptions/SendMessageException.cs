using System.Runtime.Serialization;
using UKU.IWEManager.Infrastructure.Exceptions;

namespace UKU.Messaging.Infrastructure.Exceptions;

public class SendMessageException : BaseException
{
    protected SendMessageException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// Exception for issues when sending emails.
    /// </summary>
    /// <param name="message">Message to show in response</param>
    /// <param name="details">Details to show in response</param>
    public SendMessageException(string message, string details)
        : base(message, details)
    {
    }

    /// <summary>
    /// Exception for issues when sending emails.
    /// </summary>
    /// <param name="message">Message to show in response</param>
    /// <param name="details">Details to show in response</param>
    /// <param name="innerException">Inner exception for the response</param>
    public SendMessageException(string message, string details, Exception innerException)
        : base(message, details, innerException)
    {
    }
}
