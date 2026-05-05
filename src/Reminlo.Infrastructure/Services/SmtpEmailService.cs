namespace Reminlo.Infrastructure.Services;

using Reminlo.Application.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

/// <summary>
/// Provides email sending functionality using SMTP.
/// </summary>
public class SmtpEmailService(IConfiguration configuration) : IEmailService
{
    /// <summary>
    /// Sends an email message to the specified recipient using SMTP configuration.
    /// </summary>
    /// <param name="to">The recipient email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="body">The email body content.</param>
    /// <param name="isHtml">Whether the body is HTML. Default is true.</param>
    public async Task SendAsync(string to, string subject, string body, bool isHtml = true, List<string>? cc = null, CancellationToken cancellationToken = default)
    {
        var emailConfig = configuration.GetSection("Email");

        using SmtpClient client = new(emailConfig["Host"], int.Parse(emailConfig["Port"] ?? string.Empty))
        {
            Credentials = new NetworkCredential(emailConfig["UserName"], emailConfig["Password"]),
            EnableSsl = bool.Parse(emailConfig["EnableSsl"] ?? string.Empty)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailConfig["From"] ?? string.Empty),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };
        mailMessage.To.Add(to);

        if (cc != null)
        {
            foreach (var address in cc)
            {
                mailMessage.CC.Add(address);
            }
        }
        
      
        Console.WriteLine($"Username: {emailConfig["UserName"]}");
        Console.WriteLine($"Password: {emailConfig["Password"]}");
        Console.WriteLine($"EnableSsl: {emailConfig["EnableSsl"]}");
        Console.WriteLine($"Host: {emailConfig["Host"]}");
        Console.WriteLine($"Port: {emailConfig["Port"]}");

        await client.SendMailAsync(mailMessage,cancellationToken);
    }
}
