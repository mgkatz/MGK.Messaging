using System.Threading;
using System.Threading.Tasks;

namespace UKU.Messaging.SeedWork;

public interface IEmailClient
{
    MailMessage CreateEmailMessage(MailAddress to, string subject, string body, bool isBodyHtml = false);
    MailMessage CreateEmailMessage(MailAddress from, MailAddress to, string subject, string body, bool isBodyHtml = false);
    Task<ISendMessageResult> SendMessageAsync(MailMessage mailMessage, CancellationToken cancellationToken);
}
