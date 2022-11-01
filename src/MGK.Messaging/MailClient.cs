using MGK.Acceptance;
using MGK.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UKU.Configuration.Infrastructure.Extensions;
using UKU.IWEManager.Infrastructure.Extensions;
using UKU.Messaging.Infrastructure.Enums;
using UKU.Messaging.Infrastructure.Exceptions;
using UKU.Messaging.Infrastructure.Options;

namespace UKU.Messaging;

public sealed class MailClient : IEmailClient
{
    private readonly ISmtpObject _smtpObject;
    private readonly SmtpOptions _smtpOptions;
    private readonly ILogger<MailClient> _logger;

	public MailClient(
		ISmtpObject smtpObject,
        IOptions<SmtpOptions> smtpOptions,
        ILogger<MailClient> logger)
    {
		ValidateConstructorParams(smtpObject, smtpOptions, logger);

		_smtpObject = smtpObject;
        _smtpOptions = smtpOptions.Value;
        _logger = logger;
	}

    public MailMessage CreateEmailMessage(
        MailAddress to,
        string subject,
        string body,
        bool isBodyHtml = false)
        => CreateEmailMessage(new MailAddress(_smtpOptions.UserName, "Uku - No Reply"), to, subject, body, isBodyHtml);

    public MailMessage CreateEmailMessage(
        MailAddress from,
        MailAddress to,
        string subject,
        string body,
        bool isBodyHtml = false)
    {
        Ensure.Parameter.IsNotNull(from, nameof(from));
        Ensure.Parameter.IsNotNull(to, nameof(to));
        Ensure.Parameter.IsNotNull(subject, nameof(subject));
        Ensure.Parameter.IsNotNull(body, nameof(body));
        Ensure.Value.IsNotNull(from.Address, nameof(from.Address));
        Ensure.Value.IsNotNull(to.Address, nameof(to.Address));

        return new MailMessage(from, to)
        {
            Body = body,
            BodyEncoding = Encoding.UTF8,
            DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
            IsBodyHtml = isBodyHtml,
            Subject = subject
        };
    }

    public async Task<ISendMessageResult> SendMessageAsync(
		MailMessage mailMessage,
		CancellationToken cancellationToken)
    {
		Ensure.Parameter.IsNotNull(mailMessage, nameof(mailMessage));
		byte retryNumber = 1;
		ISendMessageResult sendMessageResult;

		do
		{
            sendMessageResult = await TrySendMessageAsync(mailMessage, retryNumber, cancellationToken);

			if (sendMessageResult.Status != SendEmailStatus.Failed)
				break;

			retryNumber++;
		} while (retryNumber <= _smtpOptions.Retries);

		return sendMessageResult;
    }

	private async Task<ISendMessageResult> TrySendMessageAsync(
		MailMessage mailMessage,
		byte retryNumber,
        CancellationToken cancellationToken)
    {
		var status = SendEmailStatus.Unknown;
        SendMessageException sendMessageException = null;

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                status = SendEmailStatus.Cancelled;
                string sendEmailCancelledMessage = MessagingResources.MessagesResources.SendEmailCancelled;
                _logger.LogWarning(sendEmailCancelledMessage);
            }
			else
			{
                await _smtpObject.Client.SendMailAsync(mailMessage, cancellationToken);
                status = SendEmailStatus.Sent;
                string successMessage = MessagingResources.MessagesResources.SendEmailSuccessful.Format(mailMessage.To.ToString());
                _logger.LogInformation(successMessage);
            }
        }
		catch (Exception ex)
        {
            status = SendEmailStatus.Failed;

			if (retryNumber == _smtpOptions.Retries)
			{
                var errorMessage = MessagingResources.MessagesResources.SendEmailError.Format(mailMessage.To.ToString());
                sendMessageException = new SendMessageException(errorMessage, ex.GetExceptionMesssages(), ex);

                _logger.LogError(errorMessage);
                _logger.LogError(ex.GetExceptionMesssages());
            }
        }

		return new SendMessageResult
		{
			Error = sendMessageException,
			NumberOfRetriesMade = retryNumber,
			Status = status
		};
	}

    private static void ValidateConstructorParams(
        ISmtpObject smtpObject,
		IOptions<SmtpOptions> smtpOptions,
        ILogger<MailClient> logger)
    {
        logger.EnsureIsAlive();
		smtpOptions.EnsureIsAlive(logger);

        if (smtpObject is null)
        {
            string errorMessage = MessagingResources.MessagesResources.SmtpObjectError;
            logger.LogError(errorMessage);

            throw new ArgumentNullException(nameof(smtpObject), errorMessage);
        }
    }
}
