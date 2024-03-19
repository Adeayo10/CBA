//using System.Net.Mail;
using CBA.Models;
using MimeKit;
using MailKit.Net.Smtp;

namespace CBA.Services;
public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;

    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailConfiguration emailConfig, ILogger<EmailService> logger)
    {
        _emailConfig = emailConfig;
        _logger = logger;
        _logger.LogInformation("EmailService constructor called");
    }
   
    public async Task SendEmail(Message message)
    {
        _logger.LogInformation("SendEmail method called");
        var emailMessage = CreateEmailMessage(message);

        await SendAsync(emailMessage);
        _logger.LogInformation("Email sent successfully");
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("CBA", _emailConfig.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<p style='color:black;'>{0}</p>", message.Content) };

        _logger.LogInformation($"Email message created successfully with parameters: {emailMessage.From}, {emailMessage.To}, {emailMessage.Subject}, {emailMessage.Body}"   );
        return emailMessage;
    }

    private async Task SendAsync(MimeMessage mailMessage)
    {
        var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
            await client.SendAsync(mailMessage);
        }
        catch
        {
            throw new Exception("Email sending failed");
        }
        finally
        {
            await client.DisconnectAsync(true);
            client.Dispose();
        }
    }


}