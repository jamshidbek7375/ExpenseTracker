using ExpenseTracker.Application.Requests.Common;
using ExpenseTracker.Application.Requests.Wallet;
using ExpenseTracker.Application.Requests.WalletShare;
using ExpenseTracker.Application.ViewModels.Wallet;

namespace ExpenseTracker.Application.Stores.Interfaces;

public interface IWalletStore
{
    List<WalletViewModel> GetAll(GetWalletsRequest request);
    WalletViewModel GetById(WalletRequest request);
    WalletViewModel Create(CreateWalletRequest request);
    WalletViewModel CreateDefault(Guid userId);
    WalletViewModel Update(UpdateWalletRequest request);
    void Delete(WalletRequest request);
    void Share(CreateWalletShareRequest request);
}
