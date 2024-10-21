using ExpenseTracker.Application.Models;

namespace ExpenseTracker.Application.Services.Interfaces;

public interface IEmailService
{
    void SendWelcome(EmailMessage message);
    void SendInvitation(EmailMessage message);
    void SendWalletInvitation(EmailMessage message);
    void SendEmailConfirmation(EmailMessage message, UserInfo userInfo);
    void SendResetPassword(EmailMessage message, UserInfo userInfo);
}
