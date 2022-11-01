using UKU.Messaging.Infrastructure.Enums;
using UKU.Messaging.Infrastructure.Exceptions;

namespace UKU.Messaging.SeedWork;

public interface ISendMessageResult
{
    SendEmailStatus Status { get; init; }
    SendMessageException Error { get; init; }
    byte NumberOfRetriesMade { get; init; }
}
