using Microsoft.AspNetCore.SignalR;

namespace ExpenseTracker.Application.Hubs;

public sealed class NotificationHub : Hub
{
    public Task SendMessageAsync(WalletShareNotification message)
    {
        return Clients.All.SendAsync("WalletShareRequest", message);
    }
}

public sealed record WalletShareNotification(string OwnerName, string WalletName);
