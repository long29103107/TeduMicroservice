using System.Net;
using System.Net.Mail;
using TeduMicroservice.IDP.Common;

namespace TeduMicroservice.IDP.Services.EmailService;

public class SmtpMailService : IEmailSender
{
    private readonly EmailSMTPSettings _settings;

    public SmtpMailService(EmailSMTPSettings settings)
    {
        _settings = settings;
    }

    public void SendEmail(string recipient, string subject, string body, bool isBodyHtml = false, string sender = null)
    {
        var message = new MailMessage(_settings.From, recipient)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml,
            From = new MailAddress(_settings.From, !string.IsNullOrEmpty(sender) ? sender : _settings.From)
        };

        using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
        {
            EnableSsl = _settings.UseSsl
        };

        if(!string.IsNullOrWhiteSpace(_settings.Username) || !string.IsNullOrWhiteSpace(_settings.Password))
        {
            client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
        }    
        else
        {
            client.UseDefaultCredentials = true;
        }

        client.Send(message);
    }
}
