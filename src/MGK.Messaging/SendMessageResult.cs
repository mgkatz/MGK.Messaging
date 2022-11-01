using UKU.Messaging.Infrastructure.Enums;
using UKU.Messaging.Infrastructure.Exceptions;

namespace UKU.Messaging;

public sealed record SendMessageResult : ISendMessageResult
{
    public SendEmailStatus Status { get; init; }
    public SendMessageException Error { get; init; }
    public byte NumberOfRetriesMade { get; init; }
}
