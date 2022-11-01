namespace UKU.Messaging.Infrastructure.Options;

public sealed class SmtpOptions
{
    public const string OptionsKey = "Smtp";

    public string Client { get; set; } = string.Empty;
    public int Port { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public int Timeout { get; set; }
    public byte Retries { get; set; }
}
