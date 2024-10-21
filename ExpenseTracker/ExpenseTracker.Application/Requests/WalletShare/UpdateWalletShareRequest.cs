using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Requests.WalletShare;

internal sealed record UpdateWalletShareRequest(
    Guid UserId,
    int WalletId,
    string UsersToShareJson,
    List<string> UsersToShare)
    : CreateWalletShareRequest(
        UserId,
        WalletId,
        UsersToShareJson,
        UsersToShare);
