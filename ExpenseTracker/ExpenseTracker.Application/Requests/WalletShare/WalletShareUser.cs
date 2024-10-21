using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Requests.WalletShare;

public record WalletUserShare(Guid ShareWithUserId, WalletAccessType AccessType);
