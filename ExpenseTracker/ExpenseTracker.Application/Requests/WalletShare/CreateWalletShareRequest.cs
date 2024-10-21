using ExpenseTracker.Application.Requests.Common;

namespace ExpenseTracker.Application.Requests.WalletShare;

/// <summary>
/// Share request for Wallet.
/// </summary>
/// <param name="UserId"></param>
/// <param name="WalletId"></param>
/// <param name="UsersToShareJson">
/// This property is only needed to map list of emails from the frontend. 
/// Later it will be used to deserialize manually during model binding in 
/// CreateWalletShareFilter and mapped into <paramref name="UsersToShare"/>.
/// </param>
/// <param name="UsersToShare">
/// This property will not be sent by the client, rather it will mapped
/// using <paramref name="UsersToShareJson" />.
/// </param>
public record CreateWalletShareRequest(
    Guid UserId,
    int WalletId,
    string? UsersToShareJson,
    List<string> UsersToShare)
    : UserRequest(UserId: UserId);
