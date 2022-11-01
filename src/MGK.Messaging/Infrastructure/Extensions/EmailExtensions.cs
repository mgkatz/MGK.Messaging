using MGK.Extensions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace UKU.Messaging.Infrastructure.Extensions;

public static class EmailExtensions
{
    private static bool _IsInvalid;

    public static bool IsValidEmail(this string emailAddress)
    {
        _IsInvalid = false;

        if (emailAddress.IsNullOrEmptyOrWhiteSpace())
            return false;

        try
        {
            emailAddress = Regex.Replace(
                emailAddress,
                @"(@)(.+)$",
                DomainMapper,
                RegexOptions.None,
                TimeSpan.FromMilliseconds(200));
        }
        catch //(RegexMatchTimeoutException)
        {
            return false;
        }

        if (_IsInvalid)
            return false;

        try
        {
            return Regex.IsMatch(emailAddress,
                  @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                  RegexOptions.IgnoreCase,
                  TimeSpan.FromMilliseconds(250));
        }
        catch //(RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private static string DomainMapper(Match match)
    {
        var domainName = string.Empty;

        try
        {
            domainName = match.Groups[2].Value;
            domainName = new IdnMapping().GetAscii(domainName);
            domainName = match.Groups[1].Value + domainName;
        }
        catch
        {
            _IsInvalid = true;
        }

        return domainName;
    }
}
