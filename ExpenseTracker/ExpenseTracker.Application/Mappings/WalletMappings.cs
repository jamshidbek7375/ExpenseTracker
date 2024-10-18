using ExpenseTracker.Application.Requests.Wallet;
using ExpenseTracker.Application.ViewModels.Wallet;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Mappings;

public static class WalletMappings
{
    public static WalletViewModel ToViewModel(this Wallet wallet)
    {
        if (wallet.Owner is null || string.IsNullOrEmpty(wallet.Owner.UserName))
        {
            throw new InvalidOperationException($"Wallet owner cannot be null.");
        }

        return new WalletViewModel
        {
            Id = wallet.Id,
            Name = wallet.Name,
            Description = wallet.Description,
            Balance = wallet.Balance,
            IsMain = wallet.IsMain,
            UserId = wallet.OwnerId,
            UserName = wallet.Owner.UserName
        };
    }

    public static Wallet ToEntity(this CreateWalletRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description,
        Balance = request.Balance,
        Owner = null!,
        OwnerId = request.UserId,
    };
    
    public static Wallet ToUpdateEntity(this UpdateWalletRequest request) => new()
    {
        Id= request.Id,
        Name = request.Name,
        Description = request.Description,
        Balance = request.Balance,
        Owner = null!,
        OwnerId = request.UserId,
    };
}
