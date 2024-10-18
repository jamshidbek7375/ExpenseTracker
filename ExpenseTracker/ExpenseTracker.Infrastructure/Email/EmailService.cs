using ExpenseTracker.Application.Services.Interfaces;
using ExpenseTracker.Infrastructure.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using MimeKit.Text;

namespace ExpenseTracker.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailOptions _options;

    public EmailService(IOptionsMonitor<EmailOptions> options)
    {
        _options = options.CurrentValue;
    }

    public void SendEmail(List<MailboxAddress> to, string subject, string content)
    {

        var emailMessage = CreateEmailMessage(to, subject, content);

        Send(emailMessage);
    }

    public void SendConfirmation(string userName, string fallbackUrl)
    {
        var emailMessage = CreateEmailMessage(userName, fallbackUrl);

        Send(emailMessage);
    }

    public void SendResetPassword(string username, string fallbackUrl)
    {
        var subject = "Confirm Your Email for Expense Tracker Manager";
        var body = File.ReadAllText("D:\\.NET C#\\ExpenseTracker_\\ExpenseTracker\\ExpenseTracker.Infrastructure\\Email\\Templates\\ResetPassword.html")
                       .Replace("[UserName]", username)
                       .Replace("[FallbackUrl]", fallbackUrl);


        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Expense Tracker Manager", "noreply@expense-manager.uz"));
        message.To.Add(new MailboxAddress(username, username));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = body };

        Send(message);
    }

    private MimeMessage CreateEmailMessage(List<MailboxAddress> to, string subject, string content)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Expense Tracker", _options.From));
        emailMessage.To.AddRange(to);
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(TextFormat.Text) { Text = content };

        return emailMessage;
    }

    private MimeMessage CreateEmailMessage(string userName, string fallbackUrl)
    {
        var subject = "Confirm Your Email for Expense Tracker Manager";
        var body = File.ReadAllText("D:\\.NET C#\\ExpenseTracker_\\ExpenseTracker\\ExpenseTracker.Infrastructure\\Email\\Templates\\EmailConfirmation.html")
                       .Replace("[UserName]", userName)
                       .Replace("[ConfirmationLink]", fallbackUrl);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Expense Tracker Manager", "noreply@expense-manager.uz"));
        message.To.Add(new MailboxAddress(userName, userName));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = body };

        return message;
    }

    private void Send(MimeMessage mailMessage)
    {
        using var client = new SmtpClient();

        try
        {
            client.Connect(_options.SmtpServer, _options.Port, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(_options.Username, _options.Password);
            client.Send(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            client.Disconnect(true);
        }
    }
}
